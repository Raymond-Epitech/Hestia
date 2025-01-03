using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VersionController(IConfiguration config) : Controller
    {
        [HttpGet]
        public ActionResult GetVersion()
        {
            return Ok(config["Version"]);
        }
    }
}
