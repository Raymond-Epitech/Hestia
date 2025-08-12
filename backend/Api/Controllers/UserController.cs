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
        [HttpGet("GetByColocationId/{colocationId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<UserOutput>>> GetAllUser(Guid colocationId)
        {
            if (colocationId == Guid.Empty)
                throw new InvalidEntityException("ColocationId is empty");

            return Ok(await userService.GetAllUserAsync(colocationId));
        }

        [HttpGet("GetById/{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<UserOutput>> GetUser(Guid id)
        {
            if (id == Guid.Empty)
                throw new InvalidEntityException("ColocationId is empty");

            return Ok(await userService.GetUserAsync(id));
        }

        [HttpPut]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Guid>> UpdateUser(UserUpdate user)
        {
            if (user.Id == Guid.Empty)
                throw new InvalidEntityException("User Id is empty");

            return Ok(await userService.UpdateUserAsync(user));
        }

        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Guid>> DeleteUser(Guid id)
        {
            if (id == Guid.Empty)
                throw new InvalidEntityException("User Id is empty");

            return Ok(await userService.DeleteUserAsync(id));
        }

        [HttpPut("QuitColocation/{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Guid>> QuitColocation(Guid id)
        {
            return Ok(await userService.QuitColocationAsync(id));
        }

        [HttpPost("Register")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<UserInfo>> Register(string googleToken, UserInput userInput)
        {
            if (googleToken is "")
                throw new InvalidEntityException("Google token is empty");

            return Ok(await userService.RegisterUserAsync(googleToken, userInput));
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<UserInfo>> Login(string googleToken, string? FCMToken)
        {
            if (googleToken is "")
                throw new InvalidEntityException("Google token is empty");

            return Ok(await userService.LoginUserAsync(googleToken));
        }
    }
}
