using Business.Exceptions;
using Business.Interfaces;
using Business.Models.Input;
using Business.Models.Output;
using Business.Models.Update;
using EntityFramework.Context;
using EntityFramework.Models;
using Google.Apis.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Business.Services
{
    public class UserService(ILogger<UserService> logger,
    HestiaContext _context,
    IJwtService jwtService) : IUserService
    {
        /// <summary>
        /// Get all users from a collocation
        /// </summary>
        /// <param name="CollocationId">The Id of the collocation you want the users from</param>
        /// <returns>The list of the User in a collocation</returns>
        /// <exception cref="ContextException">An error occurred while getting all chores from the db</exception>
        public async Task<List<UserOutput>> GetAllUser(Guid CollocationId)
        {
            try
            {
                var users = await _context.User.Where(x => x.CollocationId == CollocationId).Select(x => new UserOutput
                {
                    Id = x.Id,
                    Username = x.Username,
                    Email = x.Email
                }).ToListAsync();

                logger.LogInformation($"Succes : All users from the collocation {CollocationId} found");

                return users;
            }
            catch (Exception ex)
            {
                throw new ContextException("An error occurred while getting all chores from the db", ex);
            }
        }

        /// <summary>
        /// Get a user by its ID
        /// </summary>
        /// <param name="id">The id of the user you want to get</param>
        /// <returns>The User that match the ID</returns>
        /// <exception cref="ContextException">An error occurred while getting all chores from the db</exception>
        /// <exception cref="NotFoundException">The user was not found</exception>
        public async Task<UserOutput> GetUser(Guid id)
        {
            try
            {
                var user = await _context.User.Where(x => x.Id == id).Select(x => new UserOutput
                {
                    Id = x.Id,
                    Username = x.Username,
                    Email = x.Email
                }).FirstOrDefaultAsync();

                if (user == null)
                {
                    throw new NotFoundException("User not found");
                }

                logger.LogInformation($"Succes : User {id} found");

                return user;
            }
            catch (Exception ex)
            {
                throw new ContextException("An error occurred while getting the user from the db", ex);
            }
        }

        /// <summary>
        /// Add a new user
        /// </summary>
        /// <param name="user">The user you want to add</param>
        /// <exception cref="ContextException">An error occurred while getting all chores from the db</exception>
        public async Task AddUser(UserInput user)
        {
            try
            {
                var newUser = new User
                {
                    Id = Guid.NewGuid(),
                    Username = user.Username,
                    Email = user.Email,
                    CollocationId = user.CollocationId,
                    CreatedAt = DateTime.Now.ToUniversalTime(),
                    LastConnection = DateTime.Now.ToUniversalTime(),
                    PathToProfilePicture = "default.jpg"
                };

                _context.Add(newUser);

                await _context.SaveChangesAsync();

                logger.LogInformation($"Succes : User {newUser.Id} added");
            }
            catch (Exception ex)
            {
                throw new ContextException("An error occurred while adding the user to the db", ex);
            }
        }

        /// <summary>
        /// Update a user
        /// </summary>
        /// <param name="user">The user to update with new info</param>
        /// <exception cref="NotFoundException">The user was not found</exception>
        /// <exception cref="ContextException">An error occurred while getting all chores from the db</exception>
        public async Task UpdateUser(UserUpdate user)
        {
            try
            {
                var userToUpdate = await _context.User.FirstOrDefaultAsync(x => x.Id == user.Id);

                if (userToUpdate == null)
                {
                    throw new NotFoundException("User not found");
                }

                userToUpdate.Username = user.Username;
                userToUpdate.Email = user.Email;
                userToUpdate.CollocationId = user.CollocationId;

                _context.Update(userToUpdate);

                await _context.SaveChangesAsync();

                logger.LogInformation($"Succes : User {user.Id} updated");
            }
            catch (Exception ex)
            {
                throw new ContextException("An error occurred while updating the user in the db", ex);
            }
        }

        /// <summary>
        /// To delete a user
        /// </summary>
        /// <param name="id">The id of the user you want to delete</param>
        /// <exception cref="NotFoundException">The user was not found</exception>
        /// <exception cref="ContextException">An error occurred while getting all chores from the db</exception>
        public async Task DeleteUser(Guid id)
        {
            try
            {
                var user = _context.User.Where(x => x.Id == id).FirstOrDefault();
                if (user == null)
                {
                    throw new NotFoundException($"User {id} not found");
                }
                _context.User.Remove(user);
                await _context.SaveChangesAsync();

                logger.LogInformation("Succes : User deleted");
            }
            catch (Exception ex)
            {
                throw new ContextException("An error occurred while deleting the user from the db", ex);
            }
        }

        /// <summary>
        /// Not implemented yet
        /// </summary>
        /// <param name="googleToken"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<string> LoginUser(string googleToken, string clientId)
        {
            try
            {
                var validPayload = await GoogleJsonWebSignature.ValidateAsync(googleToken);

                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, validPayload.Subject),
                    new Claim(JwtRegisteredClaimNames.Email, validPayload.Email),
                    new Claim(JwtRegisteredClaimNames.Name, validPayload.Name),
                    new Claim("picture", validPayload.Picture ?? ""),
                };

                return jwtService.GenerateToken(claims);
            }
            catch (Exception ex)
            {
                throw new ContextException("Invalid token", ex);
            }
        }
    }
}
