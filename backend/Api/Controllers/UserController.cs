using Business.Interfaces;
using Business.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Exceptions;
using Shared.Models.Input;
using Shared.Models.Output;
using Shared.Models.Update;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IUserService userService) : ControllerBase
    {
        [HttpGet("GetByColocationId/{ColocationId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<UserOutput>>> GetAllUser(Guid ColocationId)
        {
            try
            {
                var users = await userService.GetAllUserAsync(ColocationId);
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
        [Authorize]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<UserOutput>> GetUser(Guid id)
        {
            try
            {
                var user = await userService.GetUserAsync(id);
                return Ok(user);
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

        [HttpPut]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> UpdateUser(UserUpdate user)
        {
            try
            {
                await userService.UpdateUserAsync(user);
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
        [Authorize]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> DeleteUser(Guid id)
        {
            try
            {
                await userService.DeleteUserAsync(id);
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
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<UserInfo>> Register(string googleToken, UserInput userInput)
        {
            try
            {
                return Ok(await userService.RegisterUserAsync(googleToken, userInput));
            }
            catch (AlreadyExistException ex)
            {
                return Conflict(ex);
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
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<UserInfo>> Login(string googleToken)
        {
            try
            {
                return Ok(await userService.LoginUserAsync(googleToken));
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
