using Business.Exceptions;
using Business.Interfaces;
using Business.Models.Input;
using Business.Models.Output;
using Business.Models.Update;
using Microsoft.AspNetCore.Authorization;
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

        [HttpPost("/Register")]
        [AllowAnonymous]
        public async Task<ActionResult> Register(string googleToken, UserInput userInput)
        {
            try
            {
                return Ok(await userService.RegisterUser(googleToken, userInput));
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpPost("/Login")]
        [AllowAnonymous]
        public async Task<ActionResult> Login(string googleToken)
        {
            try
            {
                return Ok(await userService.LoginUser(googleToken));
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }
    }
}
