﻿using Business.Exceptions;
using Business.Interfaces;
using Business.Mappers;
using Business.Models.Input;
using Business.Models.Output;
using Business.Models.Update;
using EntityFramework.Context;
using EntityFramework.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Business.Services;

public class ChoreService(
    ILogger<ChoreService> logger,
    HestiaContext _context) : IChoreService
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
            var chores = await _context.Chore.Where(x => x.CollocationId == CollocationId).Select(x => new ChoreOutput
            {
                Id = x.Id,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt,
                DueDate = x.DueDate,
                Title = x.Title,
                Description = x.Description,
                IsDone = x.IsDone
            }).ToListAsync();
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
            var chore = await _context.Chore.Select(x => new ChoreOutput
            {
                Id = x.Id,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt,
                DueDate = x.DueDate,
                Title = x.Title,
                Description = x.Description,
                IsDone = x.IsDone
            }).FirstOrDefaultAsync(x => x.Id == id);

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

    public async Task<List<ChoreMessageOutput>> GetChoreMessageFromChoreAsync(Guid id)
    {
        try
        {
            var choreMessages = await _context.ChoreMessage.Where(x => x.ChoreId == id).Select(x => new ChoreMessageOutput
            {
                Id = x.Id,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt,
                Content = x.Content
            }).ToListAsync();

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
            await _context.Chore.AddAsync(chore);
            await _context.SaveChangesAsync();
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
            await _context.ChoreMessage.AddAsync(choreMessage);
            await _context.SaveChangesAsync();
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
            var chore = _context.Chore.Where(x => x.Id == input.Id).FirstOrDefault();
            if (chore == null)
            {
                throw new NotFoundException($"Chore {input.Id} not found");
            }

            var updatedReminder = chore.UpdateFromInput(input);
            await _context.SaveChangesAsync();

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
            var chore = _context.Chore.Where(x => x.Id == id).FirstOrDefault();
            if (chore == null)
            {
                throw new NotFoundException($"Chore {id} not found");
            }
            _context.Chore.Remove(chore);
            await _context.SaveChangesAsync();

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
            var chore = await _context.ChoreMessage.Where(x => x.ChoreId == choreId).ToListAsync();
            if (chore == null)
            {
                throw new NotFoundException($"Chore {choreId} not found or no Chore message linked to this chore");
            }
            _context.ChoreMessage.RemoveRange(chore);
            await _context.SaveChangesAsync();

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
    public async Task<List<UserOutput>> GetUserFromChore(Guid ChoreId)
    {
        try
        {
            var enroll = _context.ChoreEnrollments.Where(x => x.ChoreId == ChoreId);
            var users = await _context.User.Where(x => enroll.Select(x => x.UserId).Contains(x.Id)).Select(x => new UserOutput
            {
                Id = x.Id,
                Username = x.Username,
                Email = x.Email,
            }).ToListAsync();

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
    public async Task<List<ChoreOutput>> GetChoreFromUser(Guid UserId)
    {
        try
        {
            var enroll = _context.ChoreEnrollments.Where(x => x.UserId == UserId);
            var chores = await _context.Chore.Where(x => enroll.Select(x => x.ChoreId).Contains(x.Id)).Select(x => new ChoreOutput
            {
                Id = x.Id,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt,
                DueDate = x.DueDate,
                Title = x.Title,
                Description = x.Description,
                IsDone = x.IsDone
            }).ToListAsync();

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

            _context.ChoreEnrollments.Add(enroll);
            await _context.SaveChangesAsync();
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
    public async Task UnenrollToChore(Guid UserId, Guid ChoreId)
    {
        try
        {
            var enrollement = _context.ChoreEnrollments.Where(x => x.UserId == UserId && x.ChoreId == ChoreId).FirstOrDefault();
            if (enrollement == null)
            {
                throw new NotFoundException("No enrollement found");
            }

            _context.ChoreEnrollments.Remove(enrollement);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new ContextException("An error occurred while adding the user to the db", ex);
        }
    }
}
