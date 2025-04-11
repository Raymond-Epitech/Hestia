using Business.Interfaces;
using Business.Mappers;
using EntityFramework.Models;
using EntityFramework.Repositories;
using LazyCache;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Exceptions;
using Shared.Models.Input;
using Shared.Models.Output;
using Shared.Models.Update;

namespace Business.Services;

public class ChoreService(
    ILogger<ChoreService> logger,
    IRepository<Chore> choreRepository,
    IRepository<ChoreMessage> choreMessageRepository,
    IRepository<User> userRepository,
    IRepository<ChoreEnrollment> choreEnrollmentRepository,
    IAppCache cache) : IChoreService
{
    /// <summary>
    /// Get all Chores
    /// </summary>
    /// <returns>All the chores available</returns>
    /// <exception cref="ContextException">An error has occured while retriving the chores from db</exception>
    public async Task<List<ChoreOutput>> GetAllChoresAsync(Guid colocationId)
    {
        var cacheKey = $"chores:{colocationId}";

        return await cache.GetOrAddAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
            var chores = await choreRepository.Query()
                .Where(c => c.ColocationId == colocationId)
                .Select(c => new ChoreOutput
                {
                    Id = c.Id,
                    CreatedBy = c.CreatedBy,
                    CreatedAt = c.CreatedAt,
                    DueDate = c.DueDate,
                    Title = c.Title,
                    Description = c.Description,
                    IsDone = c.IsDone
                })
                .ToListAsync();

            logger.LogInformation($"Succes : All chores from the colocation {colocationId} found");

            return chores;
        });
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
        var chore = await choreRepository.Query()
            .Where(c => c.Id == id)
            .Select(c => new ChoreOutput
            {
                Id = c.Id,
                CreatedBy = c.CreatedBy,
                CreatedAt = c.CreatedAt,
                DueDate = c.DueDate,
                Title = c.Title,
                Description = c.Description,
                IsDone = c.IsDone
            })
            .FirstOrDefaultAsync();

        if (chore == null)
        {
            throw new NotFoundException("Chore not found");
        }

        logger.LogInformation("Succes : Chore found");
        return chore;
    }

    /// <summary>
    /// Get all chore messages from a chore
    /// </summary>
    /// <param name="choreId">The chore you want the message from</param>
    /// <returns>The list of the chore messages attaches to the chore</returns>
    /// <exception cref="NotFoundException">The chore was not found</exception>
    public async Task<List<ChoreMessageOutput>> GetChoreMessageFromChoreAsync(Guid choreId)
    {
        var choreMessages = await choreMessageRepository.Query()
            .Where(c => c.ChoreId == choreId)
            .Select(c => new ChoreMessageOutput
            {
                Id = c.Id,
                CreatedBy = c.CreatedBy,
                CreatedAt = c.CreatedAt,
                Content = c.Content
            })
            .ToListAsync();

        if (choreMessages == null)
        {
            throw new NotFoundException("No chore messages found");
        }

        logger.LogInformation("Succes : Chore messages found");

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
        
        await choreRepository.AddAsync(chore);
        await choreRepository.SaveChangesAsync();

        cache.Remove($"chores:{input.ColocationId}");

        logger.LogInformation("Succes : Chore added");
            
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
        
        await choreMessageRepository.AddAsync(choreMessage);
        await choreRepository.SaveChangesAsync();

        logger.LogInformation("Succes : Chore message added");
            
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
        var chore = await choreRepository.GetByIdAsync(input.Id);
        
        if (chore == null)
        {
            throw new NotFoundException($"Chore {input.Id} not found");
        }

        chore.UpdateFromInput(input);

        _ = choreRepository.Update(chore);
        await choreRepository.SaveChangesAsync();

        cache.Remove($"chores:{input.ColocationId}");

        logger.LogInformation("Succes : Chore updated");

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
        var chore = await choreRepository.GetByIdAsync(id);
        
        if (chore is null)
        {
            throw new NotFoundException($"Chore {id} not found");
        }

        choreRepository.Delete(chore);
        await choreRepository.SaveChangesAsync();

        cache.Remove($"chores:{chore.ColocationId}");

        logger.LogInformation("Succes : Chore deleted");

        return id;
    }

    /// <summary>
    /// Delete all chore message linked to a chore
    /// </summary>
    /// <param name="id">The id of the chore to be deleted</param>
    /// <exception cref="NotFoundException">No chore where found with this id</exception>
    /// <exception cref="ContextException">An error has occured while adding chore from db</exception>
    public async Task<Guid> DeleteChoreMessageByChoreIdAsync(Guid choreId)
    {
        await choreMessageRepository.Query()
            .Where(c => c.ChoreId == choreId)
            .ExecuteDeleteAsync();
        await choreRepository.SaveChangesAsync();

        logger.LogInformation("Succes : Chore message deleted");

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
        var users = await userRepository.Query()
            .Where(u => u.ChoreEnrollments.Any(ce => ce.ChoreId == choreId))
            .Select(u => new UserOutput
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                ColocationId = u.ColocationId
            })
            .ToListAsync();

        logger.LogInformation("Succes : Users found");

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
        var chores = await choreRepository.Query()
            .Where(c => c.ChoreEnrollments.Any(ce => ce.UserId == userId))
            .Select(c => new ChoreOutput
            {
                Id = c.Id,
                CreatedBy = c.CreatedBy,
                CreatedAt = c.CreatedAt,
                DueDate = c.DueDate,
                Title = c.Title,
                Description = c.Description,
                IsDone = c.IsDone
            })
            .ToListAsync();

        logger.LogInformation("Succes : Chores found");

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

        await choreEnrollmentRepository.AddAsync(enroll);
        await choreRepository.SaveChangesAsync();

        logger.LogInformation("Succes : User enrolled to the chore");

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
        await choreEnrollmentRepository.Query()
            .Where(ce => ce.UserId == userId && ce.ChoreId == choreId)
            .ExecuteDeleteAsync();
        await choreRepository.SaveChangesAsync();

        logger.LogInformation("Succes : User unenrolled to the chore");

        return userId;
    }
}
