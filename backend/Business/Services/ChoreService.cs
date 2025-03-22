using Business.Interfaces;
using Business.Mappers;
using EntityFramework.Models;
using EntityFramework.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using Shared.Exceptions;
using Shared.Models.Input;
using Shared.Models.Output;
using Shared.Models.Update;

namespace Business.Services;

public class ChoreService : IChoreService
{
    private readonly ILogger<ChoreService> _logger;
    private readonly IChoreRepository _choreRepository;

    public ChoreService(ILogger<ChoreService> logger, IChoreRepository choreRepository)
    {
        _logger = logger;
        _choreRepository = choreRepository;
    }

    /// <summary>
    /// Get all Chores
    /// </summary>
    /// <returns>All the chores available</returns>
    /// <exception cref="ContextException">An error has occured while retriving the chores from db</exception>
    public async Task<List<ChoreOutput>> GetAllChoresAsync(Guid ColocationId)
    {
        var chores = await _choreRepository.GetAllChoreOutputsAsync(ColocationId);
        
        _logger.LogInformation($"Succes : All chores from the colocation {ColocationId} found");
        
        return chores;
    }

    /// <summary>
    /// Get a chore
    /// </summary>
    /// <param name="id">the Guid of the chore</param>
    /// <returns>The found chore</returns>
    /// <exception cref="NotFoundException">The chore was not found</exception>
    /// <exception cref="ContextException">An error has occured while retriving chore from db</exception>
    public async Task<ChoreOutput> GetChoreAsync(Guid id)
    {
        var chore = await _choreRepository.GetChoreOutputByIdAsync(id); 

        if (chore == null)
        {
            throw new NotFoundException("Chore not found");
        }

        _logger.LogInformation("Succes : Chore found");
        return chore;
    }

    public async Task<List<ChoreMessageOutput>> GetChoreMessageFromChoreAsync(Guid choreId)
    {
        var choreMessages = await _choreRepository.GetAllChoreMessageOutputByChoreIdAsync(choreId);

        if (choreMessages == null)
        {
            throw new NotFoundException("No chore messages found");
        }

        _logger.LogInformation("Succes : Chore messages found");

        return choreMessages;
    }

    /// <summary>
    /// Add a chore
    /// </summary>
    /// <param name="input">The chore class with all info of a chore</param>
    /// <exception cref="ContextException">An error has occured while adding chore from db</exception>
    public async Task<Guid> AddChoreAsync(ChoreInput input)
    {
        var chore = input.ToDb();
        
        try
        {
            await _choreRepository.AddChoreAsync(chore);
            await _choreRepository.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while adding the chore from the db");
            throw new ContextException("An error occurred while adding the chore from the db", ex);
        }

        _logger.LogInformation("Succes : Chore added");
            
        return chore.Id;
    }

    /// <summary>
    /// Add a chore message
    /// </summary>
    /// <param name="input">The chore message class with all info of a chore message</param>
    /// <exception cref="ContextException">An error has occured while adding chore message from db</exception>
    public async Task<Guid> AddChoreMessageAsync(ChoreMessageInput input)
    {
        var choreMessage = input.ToDb();
        
        try
        {
            await _choreRepository.AddChoreMessageAsync(choreMessage);
            await _choreRepository.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while adding the chore message from the db");
            throw new ContextException("An error occurred while adding the chore message from the db", ex);
        }

        _logger.LogInformation("Succes : Chore message added");
            
        return choreMessage.Id;
    }

    /// <summary>
    /// Update a chore
    /// </summary>
    /// <param name="input">The class with all info of a chore</param>
    /// <exception cref="MissingArgumentException">The Id of the chore is missing</exception>
    /// <exception cref="NotFoundException">No chore where found with this id</exception>
    /// <exception cref="ContextException">An error has occured while adding chore from db</exception>
    public async Task<Guid> UpdateChoreAsync(ChoreUpdate input)
    {
        var chore = await _choreRepository.GetChoreByIdAsync(input.Id);
        
        if (chore == null)
        {
            throw new NotFoundException($"Chore {input.Id} not found");
        }

        try
        {
            chore.UpdateFromInput(input);
            await _choreRepository.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while updating the chore from the db");
            throw new ContextException("An error occurred while updating the chore from the db", ex);
        }

        _logger.LogInformation("Succes : Chore updated");

        return chore.Id;
    }

    /// <summary>
    /// Delete a chore
    /// </summary>
    /// <param name="id">The id of the chore to be deleted</param>
    /// <exception cref="NotFoundException">No chore where found with this id</exception>
    /// <exception cref="ContextException">An error has occured while adding chore from db</exception>
    public async Task<Guid> DeleteChoreAsync(Guid id)
    {
        var chore = await _choreRepository.GetChoreByIdAsync(id);
            
        if (chore == null)
        {
            throw new NotFoundException($"Chore {id} not found");
        }
        
        try
        {
            await _choreRepository.DeleteChoreAsync(chore);
            await _choreRepository.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while deleting the chore from the db");
            throw new ContextException("An error occurred while deleting the chore from the db", ex);
        }

        _logger.LogInformation("Succes : Chore deleted");

        return chore.Id;
    }

    /// <summary>
    /// Delete all chore message linked to a chore
    /// </summary>
    /// <param name="id">The id of the chore to be deleted</param>
    /// <exception cref="NotFoundException">No chore where found with this id</exception>
    /// <exception cref="ContextException">An error has occured while adding chore from db</exception>
    public async Task<Guid> DeleteChoreMessageByChoreIdAsync(Guid choreId)
    {
        var choreMessages = await _choreRepository.GetChoreMessageFromChoreId(choreId);
            
        if (choreMessages is null)
        {
            throw new NotFoundException($"Chore {choreId} not found or no Chore message linked to this chore");
        }
        
        try
        {
            await _choreRepository.DeleteRangeChoreMessageFromChoreId(choreMessages);
            await _choreRepository.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while deleting the chore message from the db");
            throw new ContextException("An error occurred while deleting the chore message from the db", ex);
        }

        _logger.LogInformation("Succes : Chore message deleted");

        return choreId;
    }

    /// <summary>
    /// Get all users who enrolled in a chore
    /// </summary>
    /// <param name="ChoreId">The chore</param>
    /// <returns>The list with all users who enrolled in a chore</returns>
    /// <exception cref="ContextException">An error occurred while adding the user to the db</exception>
    public async Task<List<UserOutput>> GetUserFromChore(Guid choreId)
    {
        var users = await _choreRepository.GetEnrolledUserOutputFromChoreIdAsync(choreId);

        _logger.LogInformation("Succes : Users found");

        return users;
    }

    /// <summary>
    /// Get all chores from a user
    /// </summary>
    /// <param name="UserId">Id of the user</param>
    /// <returns>The list with all chores who the user enrolled</returns>
    /// <exception cref="ContextException">An error occurred while adding the user to the db</exception>
    public async Task<List<ChoreOutput>> GetChoreFromUser(Guid userId)
    {
        var chores = await _choreRepository.GetEnrolledChoreOutputFromUserIdAsync(userId);

        _logger.LogInformation("Succes : Chores found");

        return chores;
    }

    /// <summary>
    /// Add en enrollement to the chore
    /// </summary>
    /// <param name="UserId">The id of the User</param>
    /// <param name="ChoreId">The id of the Chore</param>
    /// <exception cref="ContextException"></exception>
    public async Task<Guid> EnrollToChore(Guid UserId, Guid ChoreId)
    {
        var enroll = new ChoreEnrollment
        {
            UserId = UserId,
            ChoreId = ChoreId
        };

        try
        {
            await _choreRepository.AddChoreEnrollmentAsync(enroll);
            await _choreRepository.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while adding the user to the db");
            throw new ContextException("An error occurred while adding the user to the db", ex);
        }

        _logger.LogInformation("Succes : User enrolled to the chore");

        return enroll.ChoreId;
    }

    /// <summary>
    /// Remove en enrollement
    /// </summary>
    /// <param name="UserId">The id of the User</param>
    /// <param name="ChoreId">The id of the Chore</param>
    /// <exception cref="NotFoundException">The enrollement has not been found</exception>
    /// <exception cref="ContextException">An error occurred while adding the user to the db</exception>
    public async Task<Guid> UnenrollToChore(Guid userId, Guid choreId)
    {
        var enrollement = await _choreRepository.GetChoreEnrollmentByUserIdAndChoreIdAsync(userId, choreId);
            
        if (enrollement == null)
        {
            throw new NotFoundException("No enrollement found");
        }

        try
        {
            await _choreRepository.RemoveChoreEnrollmentAsync(enrollement);
            await _choreRepository.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while removing the user from the db");
            throw new ContextException("An error occurred while removing the user from the db", ex);
        }

        _logger.LogInformation("Succes : User unenrolled to the chore");

        return enrollement.ChoreId;
    }
}
