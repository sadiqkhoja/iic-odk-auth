using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using iic_odk_auth.Models;
using Microsoft.AspNetCore.Authorization;
using iic_odk_auth.Services.OdkService;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using System.Runtime.ConstrainedExecution;
using Microsoft.Extensions.Primitives;
using System.Net;
using static System.Net.Mime.MediaTypeNames;

namespace iic_odk_auth.Controllers;

[Authorize]
public class AuthController : Controller
{
    private readonly ILogger<AuthController> _logger;
    private readonly IOdkService odkService;
    private readonly String salt;
    private readonly Configuration _config;

    public AuthController(ILogger<AuthController> logger, IOdkService odkService, IConfiguration config)
    {
        salt = config.GetValue<String>("PasswordSalt")!;
	_config = config;
        this.odkService = odkService;
        _logger = logger;
    }

    public async Task<IActionResult> LoginAsync([FromQuery] string next)
    {
        // Create User
        _logger.LogInformation("Trying to get user from ODK");

        var user = new User()
        {
            Email = User.FindFirstValue("emails").ToLower(),
            Name = $"{User.FindFirstValue("given_name")} {User.FindFirstValue("family_name")} ({Email})",
            AzureObjectId = User.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier").ToLower(),            
        };

        user.Password = Hash(user.Email + user.AzureObjectId + salt);

        user.OdkId = await odkService.GetUserOdkIdAsync(user);

	    // var whitelistEmails = _config.GetSection("WhiteListEmails").Get<string[]>();
        // var whiteListEmail = whitelistEmails?.Where(x => string.Equals(x, user.Email, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

        // if (whiteListEmail == null)
        // {
        //     return Redirect(@"https://ikey.iicanada.ca");
        // }

        if(user.OdkId == default)
        {
            _logger.LogInformation("User not found. Creating it");
            user.OdkId = await odkService.CreateUserAsync(user);

            _logger.LogInformation("Assigning user a role on the project");
            await odkService.AddToProjectAsync(user);
        }

        _logger.LogInformation("We have the ODK user, let's login");

        var cookie = await odkService.Login(user);

        HttpContext.Response.Headers.Add("Set-Cookie", cookie);

        if (!String.IsNullOrWhiteSpace(next))
        {
            return Redirect(next);
        }

        // TODO [Medium] Maybe redirect to ikey or display some useful message
        return new ContentResult()
        {
            Content = "Ok",
            ContentType = "application/xml",
            StatusCode = 200
        };
    }

    private string Hash(string v)
    {
        return Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(v)));
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

