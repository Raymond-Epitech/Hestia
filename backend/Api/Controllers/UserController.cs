using Business.Exceptions;
using Business.Interfaces;
using Business.Models.Input;
using Business.Models.Output;
using Business.Models.Update;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IUserService userService) : ControllerBase
    {
        [HttpGet("GetByCollocationId/{CollocationId}")]
        public async Task<ActionResult<List<UserOutput>>> GetAllUser(Guid CollocationId)
        {
            try
            {
                var users = await userService.GetAllUser(CollocationId);
                return Ok(users);
            }
            catch (ContextException ex)
            {
                return UnprocessableEntity(ex);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpGet("GetById/{id}")]
        public async Task<ActionResult<UserOutput>> GetUser(Guid id)
        {
            try
            {
                var user = await userService.GetUser(id);
                return Ok(user);
            }
            catch(NotFoundException ex)
            {
                return NotFound(ex);
            }
            catch (ContextException ex)
            {
                return UnprocessableEntity(ex);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpPost]
        public async Task<ActionResult> AddUser(UserInput user)
        {
            try
            {
                await userService.AddUser(user);
                return Ok();
            }
            catch (ContextException ex)
            {
                return UnprocessableEntity(ex);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(UserUpdate user)
        {
            try
            {
                await userService.UpdateUser(user);
                return Ok();
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex);
            }
            catch (ContextException ex)
            {
                return UnprocessableEntity(ex);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(Guid id)
        {
            try
            {
                await userService.DeleteUser(id);
                return Ok();
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex);
            }
            catch (ContextException ex)
            {
                return UnprocessableEntity(ex);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpPost("/Login")]
        public async Task<ActionResult> Login(string googleToken, string clientId)
        {
            try
            {
                return Ok(await userService.LoginUser(googleToken, clientId));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }
    }
}
