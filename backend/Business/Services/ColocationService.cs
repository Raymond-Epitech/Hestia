﻿using Business.Interfaces;
using EntityFramework.Models;
using EntityFramework.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Exceptions;
using Shared.Models.Input;
using Shared.Models.Output;
using Shared.Models.Update;

namespace Business.Services
{
    public class ColocationService(ILogger<ColocationService> logger,
        IRepository<Colocation> colocationRepository,
        IRepository<User> userRepository
        ) : IColocationService
    {
        /// <summary>
        /// Get all Colocations
        /// </summary>
        /// <returns>List of all collocations</returns>
        /// <exception cref="ContextException">An error has occured while retriving the collocation from db</exception>
        public async Task<List<ColocationOutput>> GetAllColocations()
        {
            var colocations = await colocationRepository.Query()
                .Select(c => new ColocationOutput
                {
                    Id = c.Id,
                    Name = c.Name,
                    Address = c.Address,
                    Colocataires = c.Users.Select(u => u.Id).ToList()
                })
                .ToListAsync();

            logger.LogInformation("Succes : All collocation were retrived from db");
                
            return colocations;
        }

        /// <summary>
        /// Get a colocation
        /// </summary>
        /// <param name="id">Guid of a colocation</param>
        /// <returns></returns>
        /// <exception cref="NotFoundException">No colocation was found</exception>
        /// <exception cref="ContextException">An error has occured while retriving the colocation from db</exception>
        public async Task<ColocationOutput> GetColocation(Guid id)
        {
            var colocation = await colocationRepository.Query()
                .Where(c => c.Id == id)
                .Select(c => new ColocationOutput
                {
                    Id = c.Id,
                    Name = c.Name,
                    Address = c.Address,
                    Colocataires = c.Users.Select(u => u.Id).ToList()
                })
                .FirstOrDefaultAsync();

            if (colocation == null)
            {
                throw new NotFoundException($"The collocation with id {id} was not found");
            }

            logger.LogInformation($"Succes : Collocation {id} found");

            return colocation;
        }

        /// <summary>
        /// Add a new colocation
        /// </summary>
        /// <param name="colocation">The object colocation you want to add</param>
        /// <param name="AddedBy"> The User that added the colocation</param>
        /// <exception cref="ContextException">An error has occured while retriving the colocation from db</exception>
        public async Task<Guid> AddColocation(ColocationInput colocation, Guid? AddedBy)
        {
            var newColocation = new Colocation
            {
                Id = Guid.NewGuid(),
                Name = colocation.Name,
                Address = colocation.Address,
                CreatedAt = DateTime.Now.ToUniversalTime(),
                CreatedBy = colocation.CreatedBy
            };

            if (AddedBy is not null && AddedBy != Guid.Empty)
            {
                var user = await userRepository.GetByIdAsync(AddedBy!.Value);

                if (user is null)
                {
                    throw new NotFoundException($"The user {AddedBy} who created the colocation do not exist");
                }

                user.ColocationId = newColocation.Id;
                userRepository.Update(user);

                logger.LogInformation($"Succes : User {user.Id} added to colocation {newColocation.Id}");
            }

            await colocationRepository.AddAsync(newColocation);
            await colocationRepository.SaveChangesAsync();

            logger.LogInformation($"Succes : Colocation {newColocation.Id} added");

            return newColocation.Id;
        }

        /// <summary>
        /// Update a colocation
        /// </summary>
        /// <param name="colocation">The object you want to update (cannot change ID)</param>
        /// <returns></returns>
        /// <exception cref="NotFoundException">The colocation was not found</exception>
        /// <exception cref="ContextException">An error has occured while retriving the collocation from db</exception>
        public async Task<Guid> UpdateColocation(ColocationUpdate colocation)
        {
            var colocationToUpdate = await colocationRepository.GetByIdAsync(colocation.Id);
                
            if (colocationToUpdate == null)
            {
                throw new NotFoundException($"The collocation with id {colocation.Id} was not found");
            }

            colocationToUpdate.Name = colocation.Name;
            colocationToUpdate.Address = colocation.Address;

            colocationRepository.Update(colocationToUpdate);
            await colocationRepository.SaveChangesAsync();

            logger.LogInformation($"Succes : Colocation {colocation.Id} updated");

            return colocation.Id;
        }

        /// <summary>
        /// Delete a colocation
        /// </summary>
        /// <param name="id">The id of the colocation you want to delete</param>
        /// <returns></returns>
        /// <exception cref="NotFoundException">The colocation was not found</exception>
        /// <exception cref="ContextException">An error has occured while retriving the colocation from db</exception>
        public async Task<Guid> DeleteColocation(Guid colocationId)
        {
            await colocationRepository.DeleteFromIdAsync(colocationId);
            await colocationRepository.SaveChangesAsync();
                
            logger.LogInformation($"Succes : Colocation {colocationId} deleted");

            return colocationId;
        }
    }
}
