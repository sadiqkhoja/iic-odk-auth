using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using iic_odk_auth.Services.OdkService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace iic_odk_auth.Controllers
{
    public class MediaController : Controller
    {
        private readonly ILogger<MediaController> _logger;
        private readonly IOdkService odkService;

        public MediaController(ILogger<MediaController> logger, IOdkService odkService)
        {
            _logger = logger;
            this.odkService = odkService;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        // TODO [High] add other attributes of the user in the response
        public async Task<IActionResult> ProfileAsync()
        {
            var cookie = Request.Headers["Cookie"].First();

            _logger.LogInformation("cookie" + cookie);

            var user = await odkService.GetCurrentUserAsync(cookie);

            return new ContentResult()
            {
                Content = $"<root><name>{user.Name}</name></root>",
                ContentType = "application/xml",
                StatusCode = 200
            };            
        }

        // TODO [High] Fetch user family information from iKey API
        public IActionResult Family()
        {
            return new ContentResult()
            {
                Content = @"
<root>
    <item>       
        <name>1</name>
        <label>Dont fill this Survey</label>
    </item>
    <item>        
        <name>2</name>
        <label>Dont fill this Survey 2</label>
    </item>
</root>
",
                ContentType = "application/xml",
                StatusCode = 200
            };
        }
    }
}

