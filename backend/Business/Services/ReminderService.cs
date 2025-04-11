using Business.Interfaces;
using Business.Mappers;
using EntityFramework.Models;
using EntityFramework.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Exceptions;
using Shared.Models.Input;
using Shared.Models.Output;
using Shared.Models.Update;

namespace Business.Services;

public class ReminderService(ILogger<ReminderService> _logger,
    IRepository<Reminder> _reminderRepository) : IReminderService
{
    /// <summary>
    /// Get all reminders
    /// </summary>
    /// <returns>All the reminders available</returns>
    /// <exception cref="ContextException">An error has occured while retriving the reminders from db</exception>
    public async Task<List<ReminderOutput>> GetAllRemindersAsync(Guid collocationId)
    {
        var reminders = await _reminderRepository.Query()
            .Where(r => r.ColocationId == collocationId)
            .Select(r => new ReminderOutput
            {
                Id = r.Id,
                Content = r.Content,
                Color = r.Color,
                CreatedBy = r.CreatedBy,
                CreatedAt = r.CreatedAt,
                CoordX = r.CoordX,
                CoordY = r.CoordY,
                CoordZ = r.CoordZ
            })
            .ToListAsync();
            
        _logger.LogInformation("Succes : All reminders found");
            
        return reminders;
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
        var reminder = await _reminderRepository.Query()
            .Where(r => r.Id == id)
            .Select(r => new ReminderOutput
            {
                Id = r.Id,
                Content = r.Content,
                Color = r.Color,
                CreatedBy = r.CreatedBy,
                CreatedAt = r.CreatedAt,
                CoordX = r.CoordX,
                CoordY = r.CoordY,
                CoordZ = r.CoordZ
            })
            .FirstOrDefaultAsync();

        if (reminder == null)
        {
            throw new NotFoundException($"Reminder {id} not found");
        }

        _logger.LogInformation("Succes : Reminder found");
            
        return reminder;
    }

    /// <summary>
    /// Add a reminder
    /// </summary>
    /// <param name="input">The reminder class with all info of a reminder</param>
    /// <exception cref="ContextException">An error has occured while adding reminder from db</exception>
    public async Task<Guid> AddReminderAsync(ReminderInput input)
    {
        var reminder = input.ToDb();
            
        await _reminderRepository.AddAsync(reminder);
        await _reminderRepository.SaveChangesAsync();

        _logger.LogInformation("Succes : Reminder added");
            
        return reminder.Id;
    }

    /// <summary>
    /// Update a reminder
    /// </summary>
    /// <param name="input">The reminder class with all info of a reminder</param>
    /// <exception cref="MissingArgumentException">The Id of the reminder is missing</exception>
    /// <exception cref="NotFoundException">No reminder where found with this id</exception>
    /// <exception cref="ContextException">An error has occured while adding reminder from db</exception>
    public async Task<Guid> UpdateReminderAsync(ReminderUpdate input)
    {
        var reminder = await _reminderRepository.GetByIdAsync(input.Id);
            
        if (reminder == null)
        {
            throw new NotFoundException($"Reminder {input.Id} not found");
        }

        reminder.UpdateFromInput(input);
        _reminderRepository.Update(reminder);
        await _reminderRepository.SaveChangesAsync();

        _logger.LogInformation("Succes : Reminder updated");

        return reminder.Id;
    }

    /// <summary>
    /// Update a range of reminders
    /// </summary>
    /// <param name="inputs">a dictionary with the id, and the value of the reminders</param>
    /// <exception cref="NotFoundException">One or more reminder where not found with those id</exception>
    /// <exception cref="ContextException">An error has occured while adding one or more reminder from db</exception>
    public async Task<int> UpdateRangeReminderAsync(List<ReminderUpdate> inputs)
    {
        var idsToMatch = inputs.Select(x => x.Id).ToList();
        var reminders = await _reminderRepository.Query()
            .Where(r => idsToMatch.Contains(r .Id))
            .ToListAsync();
            
        if (reminders.Count == 0)
        {
            throw new NotFoundException($"No reminders found");
        }
        
        var notfounds = inputs.Select(x => x.Id).Except(reminders.Select(x => x.Id)).ToList();
            
        if (notfounds.Any())
        {
            throw new NotFoundException($"Reminders {String.Join(", ", notfounds)} was/were not found");
        }

        using (var transaction = await _reminderRepository.BeginTransactionAsync())
        {
            try
            {
                _logger.LogInformation("Transaction begin");
                    
                foreach (var reminder in reminders)
                {
                    var input = inputs.FirstOrDefault(x => x.Id == reminder.Id);
                        
                    if (input is null)
                    {
                        throw new NotFoundException($"input {reminder.Id} not found");
                    }

                    reminder.UpdateFromInput(input);
                }
                    
                await _reminderRepository.SaveChangesAsync();
                transaction.Commit();

                _logger.LogInformation("Transaction commit");
            }
            catch (NotFoundException)
            {
                _logger.LogError("Reminder not found, Transaction rollbacked");
                transaction.Rollback();
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the reminder from the db, Transaction rollbacked");
                transaction.Rollback();
                throw new ContextException("An error occurred while updating the reminder from the db", ex);
            }
        }

        _logger.LogInformation("Succes : Reminders all updated");

        return reminders.Count;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <exception cref="NotFoundException">No reminder where found with this id</exception>
    /// <exception cref="ContextException">An error has occured while adding reminder from db</exception>
    public async Task<Guid> DeleteReminderAsync(Guid id)
    {
        await _reminderRepository.DeleteFromIdAsync(id);
        await _reminderRepository.SaveChangesAsync();

        _logger.LogInformation("Succes : Reminder deleted");

        return id;
    }
}
