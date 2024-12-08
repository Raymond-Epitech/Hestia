using Business.Exceptions;
using Business.Interfaces;
using Business.Mappers;
using Business.Models.Input;
using Business.Models.Output;
using EntityFramework.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Business.Services;

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
                Color = x.Color,
                CoordX = x.CoordX,
                CoordY = x.CoordY,
                CoordZ = x.CoordZ
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
                Color = x.Color,
                CoordX = x.CoordX,
                CoordY = x.CoordY,
                CoordZ = x.CoordZ
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
    public async Task UpdateReminderAsync(ReminderUpdate input)
    {
        try
        {
            var reminder = _context.Reminder.Where(x => x.Id == input.Id).FirstOrDefault();
            if (reminder == null)
            {
                throw new NotFoundException($"Reminder {input.Id} not found");
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
    /// Update a range of reminders
    /// </summary>
    /// <param name="inputs">a dictionary with the id, and the value of the reminders</param>
    /// <exception cref="NotFoundException">One or more reminder where not found with those id</exception>
    /// <exception cref="ContextException">An error has occured while adding one or more reminder from db</exception>
    public async Task UpdateRangeReminderAsync(List<ReminderUpdate> inputs)
    {
        try
        {
            var idsToMatch = inputs.Select(x => x.Id).ToList();
            var reminders = await _context.Reminder.Where(x => idsToMatch.Contains(x.Id)).ToListAsync();
            if (reminders == null)
            {
                throw new NotFoundException($"No reminders found");
            }
            var notfounds = inputs.Select(x => x.Id).Except(reminders.Select(x => x.Id)).ToList();
            if (notfounds.Any())
            {
                throw new NotFoundException($"Reminders {String.Join(", ", notfounds)} was/were not found");
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    foreach (var reminder in reminders)
                    {
                        var input = inputs.FirstOrDefault(x => x.Id == reminder.Id);
                        reminder.UpdateFromInput(input);
                    }
                    await _context.SaveChangesAsync();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new ContextException("An error occurred while updating the reminder from the db", ex);
                }
            }

            logger.LogInformation("Succes : Reminders all updated");
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
