using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VersionController(IConfiguration config) : Controller
    {
        [HttpGet]
        [AllowAnonymous]
        public ActionResult GetVersion()
        {
            return Ok(config["Version"]);
        }
    }
}
