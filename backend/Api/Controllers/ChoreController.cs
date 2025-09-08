using Business.Interfaces;
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
    public class ChoreController(IChoreService choreService) : ControllerBase
    {
        [HttpGet("GetByColocationId/{colocationId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ChoreOutput>>> GetAllChores(Guid colocationId)
        {
            if (colocationId == Guid.Empty)
                throw new InvalidEntityException("ColocationId is empty");

            return Ok(await choreService.GetAllChoresAsync(colocationId));
        }

        [HttpGet("GetById/{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ChoreOutput>> GetChore(Guid id)
        {
            if (id == Guid.Empty)
                throw new InvalidEntityException("Chore Id is empty");

            return Ok(await choreService.GetChoreAsync(id));
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Guid>> AddChore(ChoreInput input)
        {
            return Ok(await choreService.AddChoreAsync(input));
        }

        [HttpPut]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Guid>> UpdateChore(ChoreUpdate input)
        {
            if (input.Id == Guid.Empty)
                throw new InvalidEntityException("Chore Id is empty");

            return Ok(await choreService.UpdateChoreAsync(input));
        }

        [HttpPut("Done")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Guid>> MarkChoreAsDone(Guid id)
        {
            if (id == Guid.Empty)
                throw new InvalidEntityException("Chore Id is empty");

            return Ok(await choreService.MarkChoreAsDoneAsync(id));
        }

        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Guid>> DeleteChore(Guid id)
        {
            if (id == Guid.Empty)
                throw new InvalidEntityException("Chore Id is empty");

            return Ok(await choreService.DeleteChoreAsync(id));
        }

        [HttpGet("Enroll/ByUser")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ChoreOutput>>> GetChoreFromUser(Guid UserId)
        {
            if (UserId == Guid.Empty)
                throw new InvalidEntityException("User Id is empty");

            return Ok(await choreService.GetChoreFromUser(UserId));
        }

        [HttpGet("Enroll/ByChore")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<UserOutput>>> GetUserFromChore(Guid ChoreId)
        {
            if (ChoreId == Guid.Empty)
                throw new InvalidEntityException("Chore Id is empty");

            return Ok(await choreService.GetUserFromChore(ChoreId));
        }

        [HttpPost("Enroll")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Guid>> EnrollToChore(Guid UserId, Guid ChoreId)
        {
            if (UserId == Guid.Empty)
                throw new InvalidEntityException("User Id is empty");
            if (ChoreId == Guid.Empty)
                throw new InvalidEntityException("Chore Id is empty");

            return Ok(await choreService.EnrollToChore(UserId, ChoreId));
        }

        [HttpDelete("Enroll")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Guid>> UnenrollToChore(Guid UserId, Guid ChoreId)
        {
            if (UserId == Guid.Empty)
                throw new InvalidEntityException("User Id is empty");
            if (ChoreId == Guid.Empty)
                throw new InvalidEntityException("Chore Id is empty");

            return Ok(await choreService.UnenrollToChore(UserId, ChoreId));
        }
    }
}
