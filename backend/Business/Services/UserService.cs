using Business.Interfaces;
using Business.Jwt;
using EntityFramework.Models;
using Google.Apis.Auth;
using Microsoft.Extensions.Logging;
using Shared.Exceptions;
using Shared.Models.Input;
using Shared.Models.Output;
using Shared.Models.Update;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Business.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;

        public UserService(ILogger<UserService> logger, IUserRepository userRepository, IJwtService jwtService)
        {
            _logger = logger;
            _userRepository = userRepository;
            _jwtService = jwtService;
        }

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
                var users = await _userRepository.GetAllUserOutputAsync(CollocationId);

                _logger.LogInformation($"Succes : All users from the collocation {CollocationId} found");

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
                var user = await _userRepository.GetUserOutputByIdAsync(id);

                if (user == null)
                {
                    throw new NotFoundException("User not found");
                }

                _logger.LogInformation($"Succes : User {id} found");

                return user;
            }
            catch (Exception ex)
            {
                throw new ContextException("An error occurred while getting the user from the db", ex);
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
                var userToUpdate = await _userRepository.GetUserByIdAsync(user.Id);

                if (userToUpdate == null)
                {
                    throw new NotFoundException("User not found");
                }

                userToUpdate.Username = user.Username;
                userToUpdate.ColocationId = user.ColocationId;

                await _userRepository.SaveChangesAsync();

                _logger.LogInformation($"Succes : User {user.Id} updated");
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
                var user = await _userRepository.GetUserByIdAsync(id);
                if (user == null)
                {
                    throw new NotFoundException($"User {id} not found");
                }

                await _userRepository.RemoveAsync(user);

                await _userRepository.SaveChangesAsync();

                _logger.LogInformation("Succes : User deleted");
            }
            catch (Exception ex)
            {
                throw new ContextException("An error occurred while deleting the user from the db", ex);
            }
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        /// <param name="googleToken">the token from the google API OAuth 2</param>
        /// <param name="userInput">The info for the new user</param>
        /// <returns>The JWT and the new user's info</returns>
        /// <exception cref="ContextException">Error in the DB or context</exception>
        /// <exception cref="NotFoundException">The user was not found</exception>
        public async Task<UserInfo> RegisterUser(string googleToken, UserInput userInput)
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

                if (await _userRepository.AnyExistingUserByEmail(validPayload.Email))
                {
                    throw new AlreadyExistException("This user already exist with this email");
                }

                // Add new verified user in DB
                var newUser = new User
                {
                    Id = Guid.NewGuid(),
                    Username = userInput.Username,
                    Email = validPayload.Email,
                    ColocationId = userInput.ColocationId,
                    CreatedAt = DateTime.Now.ToUniversalTime(),
                    LastConnection = DateTime.Now.ToUniversalTime(),
                    PathToProfilePicture = "default.jpg"
                };

                try
                {
                    await _userRepository.AddAsync(newUser);

                    await _userRepository.SaveChangesAsync();

                    _logger.LogInformation($"Succes : User {newUser.Id} added");
                }
                catch (Exception ex)
                {
                    throw new ContextException("An error occurred while adding the user to the db", ex);
                }

                // Generate and return JWT

                var jwt = _jwtService.GenerateToken(claims);

                var user = await _userRepository.GetUserOutputByIdAsync(newUser.Id);

                if (user is null)
                {
                    throw new NotFoundException("User not found");
                }

                var userInfo = new UserInfo
                {
                    Jwt = jwt,
                    User = user
                };
                
                return userInfo;
            }
            catch (Exception ex)
            {
                throw new Exception("Invalid token", ex);
            }
        }

        /// <summary>
        /// Not implemented yet
        /// </summary>
        /// <param name="googleToken"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<UserInfo> LoginUser(string googleToken)
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

                var user = await _userRepository.GetUserByEmailAsync(validPayload.Email);

                if (user is null)
                {
                    throw new NotFoundException("User not found");
                }
                else
                {
                    try
                    {
                        user.LastConnection = DateTime.UtcNow;
                        await _userRepository.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        throw new ContextException("Error while updating the user", ex);
                    }
                }

                // Generate and return JWT

                var jwt = _jwtService.GenerateToken(claims);

                var userOutput = new UserOutput
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    ColocationId = user.ColocationId
                };

                var userInfo = new UserInfo
                {
                    Jwt = jwt,
                    User = userOutput
                };

                return userInfo;
            }
            catch (Exception ex)
            {
                throw new ContextException("Invalid token", ex);
            }
        }
    }
}
