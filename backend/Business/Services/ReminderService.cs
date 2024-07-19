using Business.Exceptions;
using Business.Interfaces;
using Business.Models.Output;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyConsoleApp.Models;

namespace BookStoreApi.Services;

public class ReminderService(
    ILogger<ReminderService> logger,
    HestiaContext context) : IReminderService
{
    /// <summary>
    /// Get a reminder from db
    /// </summary>
    /// <param name="id">the Guid of the reminder</param>
    /// <returns>The found reminder</returns>
    /// <exception cref="NotFoundException">The reminder was not found</exception>
    /// <exception cref="RetreivingException">An error has occured while retriving reminder from db</exception>
    public async Task<ReminderOutput> GetReminderAsync(Guid id)
    {
        try
        {
            var reminder = await context.Reminder.Select(x => new ReminderOutput
            {
                Id = x.Id,
                Content = x.Content,
                Color = x.Color
            }).FirstOrDefaultAsync(x => Equals(x.Id, id));

            if (reminder == null)
            {
                throw new NotFoundException("Reminder not found");
            }

            logger.LogInformation("Succes : Reminder found");
            return reminder;
        }
        catch (Exception ex)
        {
            throw new RetreivingException("An error occurred while retreiving the reminder from the db", ex);
        }
    }
}
