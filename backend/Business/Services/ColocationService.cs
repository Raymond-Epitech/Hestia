using Business.Interfaces;
using EntityFramework.Models;
using EntityFramework.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Exceptions;
using Shared.Models.Input;
using Shared.Models.Output;
using Shared.Models.Update;

namespace Business.Services
{
    public class ColocationService(
        ILogger<ColocationService> logger,
        IColocationRepository colocationRepository,
        IUserRepository userRepository) : IColocationService
    {
        /// <summary>
        /// Get all Colocations
        /// </summary>
        /// <returns>List of all collocations</returns>
        /// <exception cref="ContextException">An error has occured while retriving the collocation from db</exception>
        public async Task<List<ColocationOutput>> GetAllColocations()
        {
            try
            {
                var colocations = await colocationRepository.GetAllColocationOutputAsync();

                logger.LogInformation("Succes : All collocation were retrived from db");
                
                return colocations;
            }
            catch (Exception ex)
            {
                throw new ContextException("An error occurred while getting all collocations from the db", ex);
            }
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
            try
            {
                var colocation = await colocationRepository.GetColocationOutputFromIdAsync(id);

                if (colocation == null)
                {
                    throw new NotFoundException($"The collocation with id {id} was not found");
                }

                logger.LogInformation($"Succes : Collocation {id} found");

                return colocation;
            }
            catch (Exception ex)
            {
                throw new ContextException("An error occurred while getting the collocation from the db", ex);
            }
        }

        /// <summary>
        /// Add a new colocation
        /// </summary>
        /// <param name="colocation">The object colocation you want to add</param>
        /// <param name="AddedBy"> The User that added the colocation</param>
        /// <exception cref="ContextException">An error has occured while retriving the colocation from db</exception>
        public async Task<Guid> AddCollocation(ColocationInput colocation, Guid? AddedBy)
        {
            try
            {
                var newColocation = new Colocation
                {
                    Id = Guid.NewGuid(),
                    Name = colocation.Name,
                    Address = colocation.Address,
                    CreatedAt = DateTime.Now.ToUniversalTime(),
                    CreatedBy = colocation.CreatedBy
                };

                await colocationRepository.AddColocationAsync(newColocation);

                if (AddedBy != Guid.Empty)
                {
                    var user = await userRepository.GetUserByIdAsync(AddedBy!.Value);

                    if (user is null)
                    {
                        throw new NotFoundException($"The user {AddedBy} who created the colocation do not exist");
                    }

                    user.ColocationId = newColocation.Id;
                    await userRepository.UpdateAsync(user);
                }

                await colocationRepository.SaveChangesAsync();

                logger.LogInformation($"Succes : Colocation {newColocation.Id} added");

                return newColocation.Id;
            }
            catch (Exception ex)
            {
                throw new ContextException("An error occurred while adding the colocation to the db", ex);
            }
        }

        /// <summary>
        /// Update a colocation
        /// </summary>
        /// <param name="colocation">The object you want to update (cannot change ID)</param>
        /// <returns></returns>
        /// <exception cref="NotFoundException">The colocation was not found</exception>
        /// <exception cref="ContextException">An error has occured while retriving the collocation from db</exception>
        public async Task UpdateColocation(ColocationUpdate colocation)
        {
            try
            {
                var colocationToUpdate = await colocationRepository.GetColocationFromIdAsync(colocation.Id);
                if (colocationToUpdate == null)
                {
                    throw new NotFoundException($"The collocation with id {colocation.Id} was not found");
                }

                colocationToUpdate.Name = colocation.Name;
                colocationToUpdate.Address = colocation.Address;

                await colocationRepository.SaveChangesAsync();
                logger.LogInformation($"Succes : Colocation {colocation.Id} updated");
            }
            catch (Exception ex)
            {
                throw new ContextException("An error occurred while updating the colocation in the db", ex);
            }
        }

        /// <summary>
        /// Delete a colocation
        /// </summary>
        /// <param name="id">The id of the colocation you want to delete</param>
        /// <returns></returns>
        /// <exception cref="NotFoundException">The colocation was not found</exception>
        /// <exception cref="ContextException">An error has occured while retriving the colocation from db</exception>
        public async Task DeleteColocation(Guid colocationId)
        {
            try
            {
                var colocation = await colocationRepository.GetColocationFromIdAsync(colocationId);
                if (colocation == null)
                {
                    throw new NotFoundException($"The colocation with id {colocationId} was not found");
                }

                await colocationRepository.DeleteColocationAsync(colocation);
                await colocationRepository.SaveChangesAsync();
                logger.LogInformation($"Succes : Colocation {colocationId} deleted");
            }
            catch (Exception ex)
            {
                throw new ContextException("An error occurred while deleting the colocation from the db", ex);
            }
        }
    }
}
