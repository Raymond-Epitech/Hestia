using Business.Interfaces;
using EntityFramework.Models;
using EntityFramework.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using Shared.Exceptions;
using Shared.Models.Input;
using Shared.Models.Output;
using Shared.Models.Update;

namespace Business.Services
{
    public class ColocationService : IColocationService
    {
        private readonly ILogger<ChoreService> _logger;
        private readonly IColocationRepository _colocationRepository;
        private readonly IUserRepository _userRepository;

        public ColocationService(ILogger<ChoreService> logger, IColocationRepository colocationRepository, IUserRepository userRepository)
        {
            _logger = logger;
            _colocationRepository = colocationRepository;
            _userRepository = userRepository;
        }
        /// <summary>
        /// Get all Colocations
        /// </summary>
        /// <returns>List of all collocations</returns>
        /// <exception cref="ContextException">An error has occured while retriving the collocation from db</exception>
        public async Task<List<ColocationOutput>> GetAllColocations()
        {
            try
            {
                var colocations = await _colocationRepository.GetAllColocationOutputAsync();

                _logger.LogInformation("Succes : All collocation were retrived from db");
                
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
                var colocation = await _colocationRepository.GetColocationOutputFromIdAsync(id);

                if (colocation == null)
                {
                    throw new NotFoundException($"The collocation with id {id} was not found");
                }

                _logger.LogInformation($"Succes : Collocation {id} found");

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
        public async Task<Guid> AddColocation(ColocationInput colocation, Guid? AddedBy)
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

                await _colocationRepository.AddColocationAsync(newColocation);

                if (AddedBy != Guid.Empty)
                {
                    var user = await _userRepository.GetUserByIdAsync(AddedBy!.Value);

                    if (user is null)
                    {
                        throw new NotFoundException($"The user {AddedBy} who created the colocation do not exist");
                    }

                    user.ColocationId = newColocation.Id;
                    await _userRepository.UpdateAsync(user);
                }

                await _colocationRepository.SaveChangesAsync();

                _logger.LogInformation($"Succes : Colocation {newColocation.Id} added");

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
                var colocationToUpdate = await _colocationRepository.GetColocationFromIdAsync(colocation.Id);
                if (colocationToUpdate == null)
                {
                    throw new NotFoundException($"The collocation with id {colocation.Id} was not found");
                }

                colocationToUpdate.Name = colocation.Name;
                colocationToUpdate.Address = colocation.Address;

                await _colocationRepository.SaveChangesAsync();
                _logger.LogInformation($"Succes : Colocation {colocation.Id} updated");
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
                var colocation = await _colocationRepository.GetColocationFromIdAsync(colocationId);
                if (colocation == null)
                {
                    throw new NotFoundException($"The colocation with id {colocationId} was not found");
                }

                await _colocationRepository.DeleteColocationAsync(colocation);
                await _colocationRepository.SaveChangesAsync();
                _logger.LogInformation($"Succes : Colocation {colocationId} deleted");
            }
            catch (Exception ex)
            {
                throw new ContextException("An error occurred while deleting the colocation from the db", ex);
            }
        }
    }
}
