using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcClient.Models;
using Newtonsoft.Json.Linq;

namespace MvcClient.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        [Authorize(AuthenticationSchemes="oidc")]   // 如不设置Scheme，默认优先使用远程认证，然后使用本地认证。在这里默认使用oidc认证，并执行Challenge方法。
        public IActionResult Contact()
        {
            var data = from c in User.Claims select new { c.Type, c.Value };
            ViewData["Message"] = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
