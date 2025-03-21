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
        try
        {
            var reminders = await _reminderRepository.GetAllReminderOutputsAsync(CollocationId);
            
            _logger.LogInformation("Succes : All reminders found");
            
            return reminders;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting all reminders from the db");
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
            var reminder = await _reminderRepository.GetReminderOutputAsync(id);

            if (reminder == null)
            {
                throw new NotFoundException($"Reminder {id} not found");
            }

            _logger.LogInformation("Succes : Reminder found");
            
            return reminder;
        }
        catch (NotFoundException)
        {
            _logger.LogError("Reminder not found");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting the reminder from the db");
            throw new ContextException("An error occurred while getting the reminder from the db", ex);
        }
    }

    /// <summary>
    /// Add a reminder
    /// </summary>
    /// <param name="input">The reminder class with all info of a reminder</param>
    /// <exception cref="ContextException">An error has occured while adding reminder from db</exception>
    public async Task<Guid> AddReminderAsync(ReminderInput input)
    {
        try
        {
            var reminder = input.ToDb();
            
            await _reminderRepository.AddReminderAsync(reminder);
            await _reminderRepository.SaveChangesAsync();
            
            _logger.LogInformation("Succes : Reminder added");
            
            return reminder.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while adding the reminder from the db");
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
            var reminder = await _reminderRepository.GetReminderAsync(input.Id);
            
            if (reminder == null)
            {
                throw new NotFoundException($"Reminder {input.Id} not found");
            }

            reminder.UpdateFromInput(input);

            await _reminderRepository.SaveChangesAsync();

            _logger.LogInformation("Succes : Reminder updated");
        }
        catch (NotFoundException)
        {
            _logger.LogError("Reminder not found");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while updating the reminder from the db");
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
            var reminders = await _reminderRepository.GetReminderFromListOfId(idsToMatch);
            
            if (reminders == null)
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
                            throw new NotFoundException($"input not found");
                        }
                        else
                        {
                            reminder.UpdateFromInput(input);
                        }
                    }
                    
                    await _reminderRepository.SaveChangesAsync();
                    transaction.Commit();

                    _logger.LogInformation("Transaction commit");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while updating the reminder from the db, Transaction rollbacked");
                    transaction.Rollback();
                    throw new ContextException("An error occurred while updating the reminder from the db", ex);
                }
            }

            _logger.LogInformation("Succes : Reminders all updated");
        }
        catch (NotFoundException)
        {
            _logger.LogError("Reminder not found");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while updating the reminder from the db");
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
            var reminder = await _reminderRepository.GetReminderAsync(id);
            
            if (reminder == null)
            {
                throw new NotFoundException($"Reminder {id} not found");
            }
            
            await _reminderRepository.DeleteReminderAsync(reminder);
            await _reminderRepository.SaveChangesAsync();

            _logger.LogInformation("Succes : Reminder deleted");
        }
        catch (NotFoundException)
        {
            _logger.LogError("Reminder not found");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting the reminder from the db");
            throw new ContextException("An error occurred while deleting the reminder from the db", ex);
        }
    }
}
