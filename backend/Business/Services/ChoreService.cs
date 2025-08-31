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
    IAppCache cache,
    IRealTimeService realTimeService) : IChoreService
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
                .Include(c => c.ChoreEnrollments)
                .ThenInclude(ce => ce.User)
                .Where(c => c.ColocationId == colocationId)
                .OrderBy(c => c.DueDate)
                .ToListAsync();

            var choreOutputs = chores
                .Select(c => c.ToOutput())
                .ToList();

            logger.LogInformation($"Succes : All chores from the colocation {colocationId} found");

            return choreOutputs;
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
        var choreEntity = await choreRepository.Query()
        .Where(c => c.Id == id)
        .Include(c => c.ChoreEnrollments)
        .ThenInclude(ce => ce.User)
        .FirstOrDefaultAsync();

        if (choreEntity is null)
            throw new NotFoundException($"Chore with id {id} not found");

        logger.LogInformation("Succes : Chore found");

        return choreEntity.ToOutput();
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
            .OrderBy(c => c.CreatedAt)
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
        if (input.Enrolled is not null && input.Enrolled.Count != 0)
        {
            await choreEnrollmentRepository.AddRangeAsync(
                input.Enrolled.Select(x => new ChoreEnrollment
                {
                    UserId = x,
                    ChoreId = chore.Id
                }).ToList());
        }
        await choreRepository.SaveChangesAsync();

        cache.Remove($"chores:{input.ColocationId}");

        chore = await choreRepository.Query()
            .Include(c => c.ChoreEnrollments)
            .ThenInclude(ce => ce.User)
            .FirstOrDefaultAsync(c => c.Id == chore.Id);

        if (chore is null)
            throw new InvalidEntityException("Could get the new chore created");

        await realTimeService.SendToGroupAsync(chore.ColocationId, "NewChoreAdded", chore.ToOutput());

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

        var choreMessageOutput = new ChoreMessageOutput
        {
            Id = choreMessage.Id,
            CreatedBy = choreMessage.CreatedBy,
            CreatedAt = choreMessage.CreatedAt,
            Content = choreMessage.Content
        };
        await realTimeService.SendToGroupAsync(input.ColocationId, "NewChoreMessageAdded", choreMessageOutput);

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

        if (input.Enrolled.Count != 0)
        {
            await choreEnrollmentRepository.Query()
                .Where(ce => ce.ChoreId == chore.Id)
                .ExecuteDeleteAsync();
            await choreEnrollmentRepository.AddRangeAsync(
                input.Enrolled.Select(x => new ChoreEnrollment
                {
                    UserId = x,
                    ChoreId = chore.Id
                }).ToList());
        }

        choreRepository.Update(chore);
        await choreRepository.SaveChangesAsync();

        cache.Remove($"chores:{input.ColocationId}");

        chore = await choreRepository.Query()
            .Include(c => c.ChoreEnrollments)
            .ThenInclude(ce => ce.User)
            .FirstOrDefaultAsync(c => c.Id == input.Id);

        if (chore is null)
            throw new InvalidEntityException("Could get the updated chore");

        await realTimeService.SendToGroupAsync(chore.ColocationId, "ChoreUpdated", chore.ToOutput());

        logger.LogInformation("Succes : Chore updated");

        return chore.Id;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="NotFoundException"></exception>
    public async Task<Guid> MarkChoreAsDoneAsync(Guid id)
    {
        var chore = await choreRepository.Query()
            .Include(c => c.ChoreEnrollments)
            .ThenInclude(ce => ce.User)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (chore is null)
        {
            throw new NotFoundException($"Chore {id} not found");
        }

        chore.IsDone = !chore.IsDone;
        chore.UpdatedAt = DateTime.UtcNow;

        choreRepository.Update(chore);
        await choreRepository.SaveChangesAsync();

        cache.Remove($"chores:{chore.ColocationId}");

        await realTimeService.SendToGroupAsync(chore.ColocationId, "ChoreUpdated", chore.ToOutput());

        logger.LogInformation("Succes : Chore marked as done/undone");

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

        await realTimeService.SendToGroupAsync(chore.ColocationId, "ChoreDeleted", id);

        logger.LogInformation("Succes : Chore deleted");

        return id;
    }

    /// <summary>
    /// Delete all chore message linked to a chore
    /// </summary>
    /// <param name="id">The id of the chore to be deleted</param>
    /// <exception cref="NotFoundException">No chore where found with this id</exception>
    /// <exception cref="ContextException">An error has occured while adding chore from db</exception>
    public async Task<Guid> DeleteChoreMessageByChoreMessageIdAsync(ChoreMessageToDelete choreMessage)
    {
        await choreMessageRepository.Query()
            .Where(c => c.Id == choreMessage.ChoreMessageId)
            .ExecuteDeleteAsync();
        await choreRepository.SaveChangesAsync();

        await realTimeService.SendToGroupAsync(choreMessage.ColocationId, "ChoreMessagesDeleted", choreMessage.ChoreMessageId);

        logger.LogInformation("Succes : Chore message deleted");

        return choreMessage.ChoreMessageId;
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
                ColocationId = u.ColocationId,
                ProfilePictureUrl = u.PathToProfilePicture
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
        var choreEntities = await choreRepository.Query()
            .Where(c => c.ChoreEnrollments.Any(ce => ce.UserId == userId))
            .Include(c => c.ChoreEnrollments)
            .ThenInclude(ce => ce.User)
            .ToListAsync();

        var chores = choreEntities
            .Select(c => c.ToOutput())
            .ToList();

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
        if (await choreEnrollmentRepository.Query().AnyAsync(e => e.UserId == UserId && e.ChoreId == ChoreId))
            throw new InvalidEntityException($"User {UserId} is already enrolled to the chore {ChoreId}");

        var enroll = new ChoreEnrollment
        {
            UserId = UserId,
            ChoreId = ChoreId
        };

        await choreEnrollmentRepository.AddAsync(enroll);
        await choreRepository.SaveChangesAsync();

        await choreRepository.Query()
            .Where(c => c.Id == enroll.ChoreId)
            .ExecuteUpdateAsync(s => s.SetProperty(c => c.UpdatedAt, DateTime.UtcNow));

        var chore = await choreRepository.Query()
            .Include(c => c.ChoreEnrollments)
            .ThenInclude(ce => ce.User)
            .FirstOrDefaultAsync(c => c.Id == ChoreId);

        if (chore is null)
            throw new InvalidEntityException($"Could get the chore with id {ChoreId}");

        cache.Remove($"chores:{chore.ColocationId}");

        await realTimeService.SendToGroupAsync(enroll.Chore.ColocationId, "ChoreEnrollmentAdded", chore.ToOutput());

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
        var enrollement = await choreEnrollmentRepository.Query()
            .Where(ce => ce.UserId == userId && ce.ChoreId == choreId)
            .Include(e => e.Chore)
            .FirstOrDefaultAsync();
        if (enrollement == null)
        {
            throw new NotFoundException("Enrollement not found");
        }

        choreEnrollmentRepository.Delete(enrollement);
        await choreEnrollmentRepository.SaveChangesAsync();

        await choreRepository.Query()
            .Where(c => c.Id == enrollement.ChoreId)
            .ExecuteUpdateAsync(s => s.SetProperty(c => c.UpdatedAt, DateTime.UtcNow));

        cache.Remove($"chores:{enrollement.Chore.ColocationId}");

        if (enrollement is null)
            throw new InvalidEntityException($"Error in deletion of enrollment user : {userId} and chore : {choreId}");

        var chore = await choreRepository.Query()
            .Include(c => c.ChoreEnrollments)
            .ThenInclude(ce => ce.User)
            .FirstOrDefaultAsync(c => c.Id == choreId);

        if (chore is null)
            throw new InvalidEntityException($"Could get the chore with id {choreId}");

        await realTimeService.SendToGroupAsync(enrollement.Chore.ColocationId, "ChoreEnrollmentRemoved", chore.ToOutput());

        logger.LogInformation("Succes : User unenrolled to the chore");

        return userId;
    }
}
