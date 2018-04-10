using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Geek.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class ValueController : Controller
    {
        [HttpGet]
        public string Get()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(User.Claims.Select(x => new { x.Type, x.Value }));
        }
    }
}