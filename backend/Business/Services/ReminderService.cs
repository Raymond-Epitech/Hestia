using Business.Interfaces;
using Business.Mappers;
using EntityFramework.Models;
using EntityFramework.Repositories;
using Microsoft.Extensions.Logging;
using Shared.Exceptions;
using Shared.Models.Input;
using Shared.Models.Output;
using Shared.Models.Update;

namespace Business.Services;

public class ReminderService(ILogger<ReminderService> _logger,
    IRepository<Reminder> _repository,
    IColocationRepository<Reminder> _colocationIdRepository) : IReminderService
{
    /// <summary>
    /// Get all reminders
    /// </summary>
    /// <returns>All the reminders available</returns>
    /// <exception cref="ContextException">An error has occured while retriving the reminders from db</exception>
    public async Task<List<ReminderOutput>> GetAllRemindersAsync(Guid collocationId)
    {
        var reminders = await _colocationIdRepository.GetAllByColocationIdAsTypeAsync(collocationId, r => new ReminderOutput
        {
            Id = r.Id,
            Content = r.Content,
            Color = r.Color,
            CreatedBy = r.CreatedBy,
            CreatedAt = r.CreatedAt,
            CoordX = r.CoordX,
            CoordY = r.CoordY,
            CoordZ = r.CoordZ
        });
            
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
        var reminder = await _repository.GetByIdAsTypeAsync(id, r => new ReminderOutput
        {
            Id = r.Id,
            Content = r.Content,
            Color = r.Color,
            CreatedBy = r.CreatedBy,
            CreatedAt = r.CreatedAt,
            CoordX = r.CoordX,
            CoordY = r.CoordY,
            CoordZ = r.CoordZ
        });

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
            
        await _repository.AddAsync(reminder);
        await _repository.SaveChangesAsync();

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
        var reminder = await _repository.GetByIdAsync(input.Id);
            
        if (reminder == null)
        {
            throw new NotFoundException($"Reminder {input.Id} not found");
        }

        reminder.UpdateFromInput(input);
        _repository.Update(reminder);
        await _repository.SaveChangesAsync();

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
        var reminders = await _repository.GetAllByList(idsToMatch);
            
        if (reminders.Count == 0)
        {
            throw new NotFoundException($"No reminders found");
        }
        
        var notfounds = inputs.Select(x => x.Id).Except(reminders.Select(x => x.Id)).ToList();
            
        if (notfounds.Any())
        {
            throw new NotFoundException($"Reminders {String.Join(", ", notfounds)} was/were not found");
        }

        using (var transaction = await _repository.BeginTransactionAsync())
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
                    
                await _repository.SaveChangesAsync();
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
        _repository.DeleteFromIdAsync(id);
        await _repository.SaveChangesAsync();

        _logger.LogInformation("Succes : Reminder deleted");

        return id;
    }
}
