using Business.Interfaces;
using Business.Mappers;
using EntityFramework.Models;
using EntityFramework.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Exceptions;
using Shared.Models.Input;
using Shared.Models.Output;
using Shared.Models.Update;

namespace Business.Services;

public class ChoreService(
    ILogger<ChoreService> logger,
    IChoreRepository choreRepository) : IChoreService
{
    /// <summary>
    /// Get all Chores
    /// </summary>
    /// <returns>All the chores available</returns>
    /// <exception cref="ContextException">An error has occured while retriving the chores from db</exception>
    public async Task<List<ChoreOutput>> GetAllChoresAsync(Guid CollocationId)
    {
        try
        {
            var chores = await choreRepository.GetAllChoreOutputsAsync(CollocationId);
            logger.LogInformation($"Succes : All chores from the collocation {CollocationId} found");
            return chores;
        }
        catch (Exception ex)
        {
            throw new ContextException("An error occurred while getting all chores from the db", ex);
        }
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
        try
        {
            var chore = await choreRepository.GetChoreOutputByIdAsync(id); 

            if (chore == null)
            {
                throw new NotFoundException("Reminder not found");
            }

            logger.LogInformation("Succes : Reminder found");
            return chore;
        }
        catch (Exception ex)
        {
            throw new ContextException("An error occurred while getting the reminder from the db", ex);
        }
    }

    public async Task<List<ChoreMessageOutput>> GetChoreMessageFromChoreAsync(Guid choreId)
    {
        try
        {
            var choreMessages = await choreRepository.GetAllChoreMessageOutputByChoreIdAsync(choreId);

            if (choreMessages == null)
            {
                throw new NotFoundException("No chore messages found");
            }

            return choreMessages;
        }
        catch (Exception ex)
        {
            throw new ContextException("An error occurred while getting the chore messages from the db", ex);
        }
    }

    /// <summary>
    /// Add a chore
    /// </summary>
    /// <param name="input">The chore class with all info of a chore</param>
    /// <exception cref="ContextException">An error has occured while adding chore from db</exception>
    public async Task<Guid> AddChoreAsync(ChoreInput input)
    {
        try
        {
            var chore = input.ToDb();
            await choreRepository.AddChoreAsync(chore);
            await choreRepository.SaveChangesAsync();
            logger.LogInformation("Succes : Chore added");
            return chore.Id;
        }
        catch (Exception ex)
        {
            throw new ContextException("An error occurred while adding the chore from the db", ex);
        }
    }

    /// <summary>
    /// Add a chore message
    /// </summary>
    /// <param name="input">The chore message class with all info of a chore message</param>
    /// <exception cref="ContextException">An error has occured while adding chore message from db</exception>
    public async Task<Guid> AddChoreMessageAsync(ChoreMessageInput input)
    {
        try
        {
            var choreMessage = input.ToDb();
            await choreRepository.AddChoreMessageAsync(choreMessage);
            await choreRepository.SaveChangesAsync();
            logger.LogInformation("Succes : Chore message added");
            return choreMessage.Id;
        }
        catch (Exception ex)
        {
            throw new ContextException("An error occurred while adding the chore message from the db", ex);
        }
    }

    /// <summary>
    /// Update a chore
    /// </summary>
    /// <param name="input">The class with all info of a chore</param>
    /// <exception cref="MissingArgumentException">The Id of the chore is missing</exception>
    /// <exception cref="NotFoundException">No chore where found with this id</exception>
    /// <exception cref="ContextException">An error has occured while adding chore from db</exception>
    public async Task UpdateChoreAsync(ChoreUpdate input)
    {
        try
        {
            var chore = await choreRepository.GetChoreByIdAsync(input.Id);
            if (chore == null)
            {
                throw new NotFoundException($"Chore {input.Id} not found");
            }

            chore.UpdateFromInput(input);
            await choreRepository.UpdateChoreAsync(chore);
            await choreRepository.SaveChangesAsync();

            logger.LogInformation("Succes : Chore updated");
        }
        catch (Exception ex)
        {
            throw new ContextException("An error occurred while updating the chore from the db", ex);
        }
    }

    /// <summary>
    /// Delete a chore
    /// </summary>
    /// <param name="id">The id of the chore to be deleted</param>
    /// <exception cref="NotFoundException">No chore where found with this id</exception>
    /// <exception cref="ContextException">An error has occured while adding chore from db</exception>
    public async Task DeleteChoreAsync(Guid id)
    {
        try
        {
            var chore = await choreRepository.GetChoreByIdAsync(id);
            if (chore == null)
            {
                throw new NotFoundException($"Chore {id} not found");
            }
            await choreRepository.DeleteChoreAsync(chore);
            await choreRepository.SaveChangesAsync();

            logger.LogInformation("Succes : Chore deleted");
        }
        catch (Exception ex)
        {
            throw new ContextException("An error occurred while deleting the chore from the db", ex);
        }
    }

    /// <summary>
    /// Delete all chore message linked to a chore
    /// </summary>
    /// <param name="id">The id of the chore to be deleted</param>
    /// <exception cref="NotFoundException">No chore where found with this id</exception>
    /// <exception cref="ContextException">An error has occured while adding chore from db</exception>
    public async Task DeleteChoreMessageByChoreIdAsync(Guid choreId)
    {
        try
        {
            var choreMessages = await choreRepository.GetChoreMessageFromChoreId(choreId);
            if (choreMessages is null)
            {
                throw new NotFoundException($"Chore {choreId} not found or no Chore message linked to this chore");
            }
            await choreRepository.DeleteRangeChoreMessageFromChoreId(choreMessages);
            await choreRepository.SaveChangesAsync();

            logger.LogInformation("Succes : Chore message deleted");
        }
        catch (Exception ex)
        {
            throw new ContextException("An error occurred while deleting the chore from the db", ex);
        }
    }

    /// <summary>
    /// Get all users who enrolled in a chore
    /// </summary>
    /// <param name="ChoreId">The chore</param>
    /// <returns>The list with all users who enrolled in a chore</returns>
    /// <exception cref="ContextException">An error occurred while adding the user to the db</exception>
    public async Task<List<UserOutput>> GetUserFromChore(Guid choreId)
    {
        try
        {
            var users = await choreRepository.GetEnrolledUserOutputFromChoreIdAsync(choreId);

            logger.LogInformation("Succes : Users found");

            return users;
        }
        catch (Exception ex)
        {
            throw new ContextException("An error occurred while adding the user to the db", ex);
        }
    }

    /// <summary>
    /// Get all chores from a user
    /// </summary>
    /// <param name="UserId">Id of the user</param>
    /// <returns>The list with all chores who the user enrolled</returns>
    /// <exception cref="ContextException">An error occurred while adding the user to the db</exception>
    public async Task<List<ChoreOutput>> GetChoreFromUser(Guid userId)
    {
        try
        {
            var chores = await choreRepository.GetEnrolledChoreOutputFromUserIdAsync(userId);

            logger.LogInformation("Succes : Chores found");

            return chores;
        }
        catch (Exception ex)
        {
            throw new ContextException("An error occurred while adding the user to the db", ex);
        }
    }

    /// <summary>
    /// Add en enrollement to the chore
    /// </summary>
    /// <param name="UserId">The id of the User</param>
    /// <param name="ChoreId">The id of the Chore</param>
    /// <exception cref="ContextException"></exception>
    public async Task EnrollToChore(Guid UserId, Guid ChoreId)
    {
        try
        {
            var enroll = new ChoreEnrollment
            {
                UserId = UserId,
                ChoreId = ChoreId
            };

            await choreRepository.AddChoreEnrollmentAsync(enroll);
            await choreRepository.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new ContextException("An error occurred while adding the user to the db", ex);
        }
    }

    /// <summary>
    /// Remove en enrollement
    /// </summary>
    /// <param name="UserId">The id of the User</param>
    /// <param name="ChoreId">The id of the Chore</param>
    /// <exception cref="NotFoundException">The enrollement has not been found</exception>
    /// <exception cref="ContextException">An error occurred while adding the user to the db</exception>
    public async Task UnenrollToChore(Guid userId, Guid choreId)
    {
        try
        {
            var enrollement = await choreRepository.GetChoreEnrollmentByUserIdAndChoreIdAsync(userId, choreId);
            if (enrollement == null)
            {
                throw new NotFoundException("No enrollement found");
            }

            await choreRepository.RemoveChoreEnrollmentAsync(enrollement);
            await choreRepository.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new ContextException("An error occurred while adding the user to the db", ex);
        }
    }
}
