using Business.Interfaces;
using Business.Mappers;
using EntityFramework.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using Shared.Exceptions;
using Shared.Models.Input;
using Shared.Models.Output;
using Shared.Models.Update;

namespace Business.Services;

public class ReminderService : IReminderService
{
    private readonly ILogger<ReminderService> _logger;
    private readonly IReminderRepository _reminderRepository;

    public ReminderService(ILogger<ReminderService> logger, IReminderRepository reminderRepository)
    {
        _logger = logger;
        _reminderRepository = reminderRepository;
    }

    /// <summary>
    /// Get all reminders
    /// </summary>
    /// <returns>All the reminders available</returns>
    /// <exception cref="ContextException">An error has occured while retriving the reminders from db</exception>
    public async Task<List<ReminderOutput>> GetAllRemindersAsync(Guid CollocationId)
    {
        var reminders = await _reminderRepository.GetAllReminderOutputsAsync(CollocationId);
            
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
        var reminder = await _reminderRepository.GetReminderOutputAsync(id);

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
            
        try
        {
            await _reminderRepository.AddReminderAsync(reminder);
            await _reminderRepository.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while adding the reminder from the db");
            throw new ContextException("An error occurred while adding the reminder from the db", ex);
        }

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
        var reminder = await _reminderRepository.GetReminderAsync(input.Id);
            
        if (reminder == null)
        {
            throw new NotFoundException($"Reminder {input.Id} not found");
        }

        try
        {
            reminder.UpdateFromInput(input);
            await _reminderRepository.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while updating the reminder from the db");
            throw new ContextException("An error occurred while updating the reminder from the db", ex);
        }

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
        var reminders = await _reminderRepository.GetReminderFromListOfId(idsToMatch);
            
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
        var reminder = await _reminderRepository.GetReminderAsync(id);
            
        if (reminder == null)
        {
            throw new NotFoundException($"Reminder {id} not found");
        }
        
        try
        {
            await _reminderRepository.DeleteReminderAsync(reminder);
            await _reminderRepository.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting the reminder from the db");
            throw new ContextException("An error occurred while deleting the reminder from the db", ex);
        }

        _logger.LogInformation("Succes : Reminder deleted");

        return reminder.Id;
    }
}
