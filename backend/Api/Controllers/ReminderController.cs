using Business.Interfaces;
using Business.Models.Output;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ReminderController(
    IReminderService reminderService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ReminderOutput>> GetReminder(Guid Id)
    {
        try
        {
            return Ok(await reminderService.GetReminderAsync(Id));
        }
        catch (Exception ex)
        {
            return NotFound(ex);
        }
    }
}
