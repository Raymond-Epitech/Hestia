using BookStoreApi.Services;
using Business.Exceptions;
using Business.Models.Output;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VersionController : Controller
    {
        [HttpGet]
        public ActionResult GetVersion()
        {
            return Ok("1.0");
        }
    }
}
