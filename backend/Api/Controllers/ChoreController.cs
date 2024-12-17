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
    public class ChoreController(IChoreService choreService) : ControllerBase
    {
        [HttpGet("GetByCollocationId/{CollocationId}")]
        public async Task<ActionResult<List<ChoreOutput>>> GetAllChores(Guid CollocationId)
        {
            try
            {
                return Ok(await choreService.GetAllChoresAsync(CollocationId));
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
        public async Task<ActionResult<ChoreOutput>> GetChore(Guid id)
        {
            try
            {
                return Ok(await choreService.GetChoreAsync(id));
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

        [HttpGet("Message/{choreId}")]
        public async Task<ActionResult<List<ChoreMessageOutput>>> GetChoreMessageFromChore(Guid choreId)
        {
            try
            {
                return Ok(await choreService.GetChoreMessageFromChoreAsync(choreId));
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

        [HttpPost]
        public async Task<ActionResult> AddChore(ChoreInput input)
        {
            try
            {
                await choreService.AddChoreAsync(input);
                return Ok();
            }
            catch (ContextException ex)
            {
                return StatusCode(500, ex);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpPost("Message/")]
        public async Task<ActionResult> AddChoreMessage(ChoreMessageInput input)
        {
            try
            {
                await choreService.AddChoreMessageAsync(input);
                return Ok();
            }
            catch (ContextException ex)
            {
                return StatusCode(500, ex);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpPut]
        public async Task<ActionResult> UpdateChore(ChoreUpdate input)
        {
            try
            {
                await choreService.UpdateChoreAsync(input);
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
        public async Task<ActionResult> DeleteChore(Guid id)
        {
            try
            {
                await choreService.DeleteChoreAsync(id);
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

        [HttpDelete("Message/{choreId}")]
        public async Task<ActionResult> DeleteChoreMessageByChoreId(Guid choreId)
        {
            try
            {
                await choreService.DeleteChoreMessageByChoreIdAsync(choreId);
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

        [HttpGet("Enroll/ByUser")]
        public async Task<ActionResult<List<ChoreOutput>>> GetChoreFromUser(Guid UserId)
        {
            try
            {
                return Ok(await choreService.GetChoreFromUser(UserId));
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

        [HttpGet("Enroll/ByChore")]
        public async Task<ActionResult<List<UserOutput>>> GetUserFromChore(Guid ChoreId)
        {
            try
            {
                return Ok(await choreService.GetUserFromChore(ChoreId));
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

        [HttpPost("Enroll")]
        public async Task<ActionResult> EnrollToChore(Guid UserId, Guid ChoreId)
        {
            try
            {
                await choreService.EnrollToChore(UserId, ChoreId);
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

        [HttpDelete("Enroll")]
        public async Task<ActionResult> UnenrollToChore(Guid UserId, Guid ChoreId)
        {
            try
            {
                await choreService.UnenrollToChore(UserId, ChoreId);
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
    }
}
