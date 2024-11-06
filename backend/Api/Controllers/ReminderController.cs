using Business.Exceptions;
using Business.Interfaces;
using Business.Models.Input;
using Business.Models.Output;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ReminderController(IReminderService reminderService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<ReminderOutput>>> GetAllReminders()
    {
        try
        {
            return Ok(await reminderService.GetAllRemindersAsync());
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

    [HttpGet("{id}")]
    public async Task<ActionResult<ReminderOutput>> GetReminder(Guid id)
    {
        try
        {
            return Ok(await reminderService.GetReminderAsync(id));
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
    public async Task<ActionResult> AddReminder(ReminderInput input)
    {
        try
        {
            await reminderService.AddReminderAsync(input);
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
    public async Task<ActionResult> DeleteReminder(Guid id)
    {
        try
        {
            await reminderService.DeleteReminderAsync(id);
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
