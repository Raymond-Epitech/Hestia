using Business.Exceptions;
using Business.Interfaces;
using Business.Mappers;
using Business.Models.Input;
using Business.Models.Output;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyConsoleApp.Models;

namespace BookStoreApi.Services;

public class ReminderService(
    ILogger<ReminderService> logger,
    HestiaContext _context) : IReminderService
{
    /// <summary>
    /// Get all reminders
    /// </summary>
    /// <returns>All the reminders available</returns>
    /// <exception cref="ContextException">An error has occured while retriving the reminders from db</exception>
    public async Task<List<ReminderOutput>> GetAllRemindersAsync()
    {
        try
        {
            var reminders = await _context.Reminder.Select(x => new ReminderOutput
            {
                Id = x.Id,
                Content = x.Content,
                Color = x.Color
            }).ToListAsync();
            logger.LogInformation("Succes : All reminders found");
            return reminders;
        }
        catch (Exception ex)
        {
            throw new ContextException("An error occurred while getting all reminders from the db", ex);
        }
    }

    /// <summary>
    /// Get a reminder
    /// </summary>
    /// <param name="id">the Guid of the reminder</param>
    /// <returns>The found reminder</returns>
    /// <exception cref="NotFoundException">The reminder was not found</exception>
    /// <exception cref="ContextException">An error has occured while retriving reminder from db</exception>
    public async Task<ReminderOutput> GetReminderAsync(Guid id)
    {
        try
        {
            var reminder = await _context.Reminder.Select(x => new ReminderOutput
            {
                Id = x.Id,
                Content = x.Content,
                Color = x.Color
            }).FirstOrDefaultAsync(x => x.Id == id);

            if (reminder == null)
            {
                throw new NotFoundException("Reminder not found");
            }

            logger.LogInformation("Succes : Reminder found");
            return reminder;
        }
        catch (Exception ex)
        {
            throw new ContextException("An error occurred while getting the reminder from the db", ex);
        }
    }

    /// <summary>
    /// Add a reminder
    /// </summary>
    /// <param name="input">The reminder class with all info of a reminder</param>
    /// <exception cref="ContextException">An error has occured while adding reminder from db</exception>
    public async Task AddReminderAsync(ReminderInput input)
    {
        try
        {
            var reminder = input.ToDb();
            await _context.Reminder.AddAsync(reminder);
            await _context.SaveChangesAsync();
            logger.LogInformation("Succes : Reminder added");
        }
        catch (Exception ex)
        {
            throw new ContextException("An error occurred while adding the reminder from the db", ex);
        }
    }

    /// <summary>
    /// Update a reminder
    /// </summary>
    /// <param name="input">The reminder class with all info of a reminder</param>
    /// <exception cref="MissingArgumentException">The Id of the reminder is missing</exception>
    /// <exception cref="NotFoundException">No reminder where found with this id</exception>
    /// <exception cref="ContextException">An error has occured while adding reminder from db</exception>
    public async Task UpdateReminderAsync(Guid id, ReminderInput input)
    {
        try
        {
            var reminder = _context.Reminder.Where(x => x.Id == id).FirstOrDefault();
            if (reminder == null)
            {
                throw new NotFoundException($"Reminder {id} not found");
            }

            var updatedReminder = reminder.UpdateFromInput(input);
            await _context.SaveChangesAsync();

            logger.LogInformation("Succes : Reminder updated");
        }
        catch (Exception ex)
        {
            throw new ContextException("An error occurred while updating the reminder from the db", ex);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <exception cref="NotFoundException">No reminder where found with this id</exception>
    /// <exception cref="ContextException">An error has occured while adding reminder from db</exception>
    public async Task DeleteReminderAsync(Guid id)
    {
        try
        {
            var reminder = _context.Reminder.Where(x => x.Id == id).FirstOrDefault();
            if (reminder == null)
            {
                throw new NotFoundException($"Reminder {id} not found");
            }
            _context.Reminder.Remove(reminder);
            await _context.SaveChangesAsync();

            logger.LogInformation("Succes : Reminder deleted");
        }
        catch (Exception ex)
        {
            throw new ContextException("An error occurred while deleting the reminder from the db", ex);
        }
    }
}
