using Business.Exceptions;
using Business.Interfaces;
using Business.Models.Input;
using Business.Models.Output;
using Business.Models.Update;
using EntityFramework.Context;
using EntityFramework.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Business.Services
{
    public class CollocationService(
        ILogger<CollocationService> logger,
        HestiaContext _context) : ICollocationService
    {
        /// <summary>
        /// Get all Collocations
        /// </summary>
        /// <returns>List of all collocations</returns>
        /// <exception cref="ContextException">An error has occured while retriving the collocation from db</exception>
        public async Task<List<CollocationOutput>> GetAllCollocations()
        {
            try
            {
                var collocations = await _context.Collocation.Select(x => new CollocationOutput
                {
                    Id = x.Id,
                    Name = x.Name,
                    Address = x.Address,
                    Collocataires = null
                }).ToListAsync();

                logger.LogInformation("Succes : All collocation were retrived from db");
                
                return collocations;
            }
            catch (Exception ex)
            {
                throw new ContextException("An error occurred while getting all collocations from the db", ex);
            }
        }

        /// <summary>
        /// Get a collocation
        /// </summary>
        /// <param name="id">Guid of a collocation</param>
        /// <returns></returns>
        /// <exception cref="NotFoundException">No collocation was found</exception>
        /// <exception cref="ContextException">An error has occured while retriving the collocation from db</exception>
        public async Task<CollocationOutput> GetCollocation(Guid id)
        {
            try
            {
                var collocation = await _context.Collocation.Where(x => x.Id == id).Select(x => new CollocationOutput
                {
                    Id = x.Id,
                    Name = x.Name,
                    Address = x.Address,
                    Collocataires = x.Users.Select(u => u.Id).ToList()
                }).FirstOrDefaultAsync();

                if (collocation == null)
                {
                    throw new NotFoundException($"The collocation with id {id} was not found");
                }

                logger.LogInformation($"Succes : Collocation {id} found");

                return collocation;
            }
            catch (Exception ex)
            {
                throw new ContextException("An error occurred while getting the collocation from the db", ex);
            }
        }

        /// <summary>
        /// Add a new collocation
        /// </summary>
        /// <param name="collocation">The object collocation you want to add</param>
        /// <param name="AddedBy"> The User that added the collocation</param>
        /// <exception cref="ContextException">An error has occured while retriving the collocation from db</exception>
        public async Task<Guid> AddCollocation(CollocationInput collocation, Guid? AddedBy)
        {
            try
            {
                var newCollocation = new Collocation
                {
                    Id = Guid.NewGuid(),
                    Name = collocation.Name,
                    Address = collocation.Address,
                    CreatedAt = DateTime.Now.ToUniversalTime(),
                    CreatedBy = collocation.CreatedBy
                };
                _context.Collocation.Add(newCollocation);
                if (AddedBy is not null)
                {
                    var user = await _context.User.FirstOrDefaultAsync(x => x.Id == AddedBy);
                    user.CollocationId = newCollocation.Id;
                    _context.User.Update(user);
                }
                await _context.SaveChangesAsync();
                logger.LogInformation($"Succes : Collocation {newCollocation.Id} added");
                return newCollocation.Id;
            }
            catch (Exception ex)
            {
                throw new ContextException("An error occurred while adding the collocation to the db", ex);
            }
        }

        /// <summary>
        /// Update a collocation
        /// </summary>
        /// <param name="collocation">The object you want to update (cannot change ID)</param>
        /// <returns></returns>
        /// <exception cref="NotFoundException">The collocation was not found</exception>
        /// <exception cref="ContextException">An error has occured while retriving the collocation from db</exception>
        public async Task UpdateCollocation(CollocationUpdate collocation)
        {
            try
            {
                var collocationToUpdate = await _context.Collocation.FirstOrDefaultAsync(x => x.Id == collocation.Id);
                if (collocationToUpdate == null)
                {
                    throw new NotFoundException($"The collocation with id {collocation.Id} was not found");
                }

                collocationToUpdate.Name = collocation.Name;
                collocationToUpdate.Address = collocation.Address;

                await _context.SaveChangesAsync();
                logger.LogInformation($"Succes : Collocation {collocation.Id} updated");
            }
            catch (Exception ex)
            {
                throw new ContextException("An error occurred while updating the collocation in the db", ex);
            }
        }

        /// <summary>
        /// Delete a collocation
        /// </summary>
        /// <param name="id">The id of the collocation you want to delete</param>
        /// <returns></returns>
        /// <exception cref="NotFoundException">The collocation was not found</exception>
        /// <exception cref="ContextException">An error has occured while retriving the collocation from db</exception>
        public async Task DeleteCollocation(Guid id)
        {
            try
            {
                var collocation = await _context.Collocation.FirstOrDefaultAsync(x => x.Id == id);
                if (collocation == null)
                {
                    throw new NotFoundException($"The collocation with id {id} was not found");
                }

                _context.Collocation.Remove(collocation);
                await _context.SaveChangesAsync();
                logger.LogInformation($"Succes : Collocation {id} deleted");
            }
            catch (Exception ex)
            {
                throw new ContextException("An error occurred while deleting the collocation from the db", ex);
            }
        }
    }
}
