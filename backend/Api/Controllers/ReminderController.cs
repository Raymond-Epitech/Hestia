using Business.Exceptions;
using Business.Interfaces;
using Business.Models.Input;
using Business.Models.Output;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ReminderController(
    IReminderService reminderService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<ReminderOutput> >> GetAllReminders()
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

    [HttpGet]
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
        catch(Exception ex)
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

    [HttpPost]
    public async Task<ActionResult> UpdateReminder(ReminderInput input)
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

    [HttpDelete]
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
