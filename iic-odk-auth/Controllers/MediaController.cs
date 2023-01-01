using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace iic_odk_auth.Controllers
{
    [Authorize]
    public class MediaController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        // TODO [High] add other attributes of the user in the response
        public IActionResult Profile()
        {
            return new ContentResult()
            {
                Content = $"<root><name>{User.FindFirstValue(ClaimTypes.GivenName)} {User.FindFirstValue(ClaimTypes.Surname)} ({User.FindFirstValue("emails").ToLower()})</name></root>",
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

