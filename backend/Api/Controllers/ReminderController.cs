using Business.Exceptions;
using Business.Interfaces;
using Business.Models.Input;
using Business.Models.Output;
using Business.Models.Update;
using EntityFramework.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReminderController(IReminderService reminderService) : ControllerBase
    {
        [Authorize]
        [HttpGet("GetByCollocation/{CollocationId}")]
        public async Task<ActionResult<List<ReminderOutput>>> GetAllReminders(Guid CollocationId)
        {
            try
            {
                return Ok(await reminderService.GetAllRemindersAsync(CollocationId));
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

        [HttpGet("GetById/{Id}")]
        [Authorize]
        public async Task<ActionResult<ReminderOutput>> GetReminder(Guid Id)
        {
            try
            {
                return Ok(await reminderService.GetReminderAsync(Id));
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
        [Authorize]
        public async Task<ActionResult<Guid>> AddReminder(ReminderInput input)
        {
            try
            {
                var id = await reminderService.AddReminderAsync(input);
                return Ok(id);
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
        [Authorize]
        public async Task<ActionResult> UpdateReminder(ReminderUpdate input)
        {
            try
            {
                await reminderService.UpdateReminderAsync(input);
                return Ok();
            }
            catch (MissingArgumentException ex)
            {
                return BadRequest(ex);
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

        [HttpPut("Range")]
        [Authorize]
        public async Task<ActionResult> UpdateRangeReminder(List<ReminderUpdate> inputs)
        {
            try
            {
                await reminderService.UpdateRangeReminderAsync(inputs);
                return Ok();
            }
            catch (MissingArgumentException ex)
            {
                return BadRequest(ex);
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
        public async Task<ActionResult> DeleteReminder(Guid Id)
        {
            try
            {
                await reminderService.DeleteReminderAsync(Id);
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