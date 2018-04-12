using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Geek.MvcClient.Controllers
{
    [Route("api/[controller]")]
    public class IdentityController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Json(User.Claims.Select(x => new { x.Type, x.Value }));
        }

        [Route("{id}")]
        [HttpGet]
        public async Task<IActionResult> Get(string id)
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            var content = await client.GetStringAsync("http://localhost.:5001/api/value");
            return Content(content);
        }
    }
}