using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult> Login(string googleToken, string clientId)
        {
            try
            {

            }
            catch (Exception ex)
            {
                StatusCode(500, ex);
            }
        }
    }
}
