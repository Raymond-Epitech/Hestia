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

namespace Business.Services;

public class UserService(ILogger<UserService> logger, IUserRepository userRepository, IJwtService jwtService) : IUserService
{
    /// <summary>
    /// Get all users from a collocation
    /// </summary>
    /// <param name="CollocationId">The Id of the collocation you want the users from</param>
    /// <returns>The list of the User in a collocation</returns>
    /// <exception cref="ContextException">An error occurred while getting all chores from the db</exception>
    public async Task<List<UserOutput>> GetAllUserAsync(Guid CollocationId)
    {
        try
        {
            var users = await userRepository.GetAllUserOutputAsync(CollocationId);

            logger.LogInformation($"Succes : All users from the collocation {CollocationId} found");

            return users;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while getting all chores from the db");
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
    public async Task<UserOutput> GetUserAsync(Guid id)
    {
        try
        {
            var user = await userRepository.GetUserOutputByIdAsync(id);

            if (user == null)
            {
                throw new NotFoundException("User not found");
            }

            logger.LogInformation($"Succes : User {id} found");

            return user;
        }
        catch (NotFoundException)
        {
            logger.LogError($"User {id} not found");
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while getting the user from the db");
            throw new ContextException("An error occurred while getting the user from the db", ex);
        }
    }

    /// <summary>
    /// Update a user
    /// </summary>
    /// <param name="user">The user to update with new info</param>
    /// <exception cref="NotFoundException">The user was not found</exception>
    /// <exception cref="ContextException">An error occurred while getting all chores from the db</exception>
    public async Task UpdateUserAsync(UserUpdate user)
    {
        try
        {
            var userToUpdate = await userRepository.GetUserByIdAsync(user.Id);

            if (userToUpdate == null)
            {
                throw new NotFoundException("User not found");
            }

            userToUpdate.Username = user.Username;
            userToUpdate.ColocationId = user.ColocationId;
            userToUpdate.PathToProfilePicture = user.PathToProfilePicture;

            await userRepository.SaveChangesAsync();

            logger.LogInformation($"Succes : User {user.Id} updated");
        }
        catch (NotFoundException)
        {
            logger.LogError($"User {user.Id} not found");
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while updating the user in the db");
            throw new ContextException("An error occurred while updating the user in the db", ex);
        }
    }

    /// <summary>
    /// To delete a user
    /// </summary>
    /// <param name="id">The id of the user you want to delete</param>
    /// <exception cref="NotFoundException">The user was not found</exception>
    /// <exception cref="ContextException">An error occurred while getting all chores from the db</exception>
    public async Task DeleteUserAsync(Guid id)
    {
        try
        {
            var user = await userRepository.GetUserByIdAsync(id);
                
            if (user == null)
            {
                throw new NotFoundException($"User {id} not found");
            }

            await userRepository.RemoveAsync(user);

            await userRepository.SaveChangesAsync();

            logger.LogInformation("Succes : User deleted");
        }
        catch (NotFoundException)
        {
            logger.LogError($"User {id} not found");
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while deleting the user from the db");
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
    /// <exception cref="AlreadyExistException">User already registered</exception>
    public async Task<UserInfo> RegisterUserAsync(string googleToken, UserInput userInput)
    {
        try
        {
            GoogleJsonWebSignature.Payload validPayload = null!;
                
            try
            {
                validPayload = await jwtService.ValidateGoogleTokenAsync(googleToken);
                logger.LogInformation("Token valid");
            }
            catch (Exception)
            {
                throw new InvalidTokenException("Google token invalid");
            }

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, validPayload.Subject),
                new Claim(JwtRegisteredClaimNames.Email, validPayload.Email),
                new Claim(JwtRegisteredClaimNames.Name, validPayload.Name),
                new Claim("picture", validPayload.Picture ?? ""),
            };

            if (await userRepository.AnyExistingUserByEmail(validPayload.Email))
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

            var newBalance = new Balance
            {
                UserId = newUser.Id,
                PersonalBalance = 0,
                LastUpdate = DateTime.Now.ToUniversalTime()
            };

            try
            {
                await userRepository.AddBalanceAsync(newBalance);
                await userRepository.AddAsync(newUser);

                await userRepository.SaveChangesAsync();

                logger.LogInformation($"Succes : User {newUser.Id} added");
            }
            catch (Exception ex)
            {
                throw new ContextException("An error occurred while adding the user to the db", ex);
            }

            // Generate and return JWT

            var jwt = jwtService.GenerateToken(claims);

            var userInfo = new UserInfo
            {
                Jwt = jwt,
                User = new UserOutput
                {
                    Id = newUser.Id,
                    Username = newUser.Username,
                    Email = newUser.Email,
                    ColocationId = newUser.ColocationId
                }
            };
                
            logger.LogInformation("Succes : User registered and JWT created");

            return userInfo;
        }
        catch (AlreadyExistException)
        {
            logger.LogError("User already exist with this email");
            throw;
        }
        catch (InvalidTokenException)
        {
            logger.LogError("Google token invalid");
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error when saving the info in the DB");
            throw new Exception("Invalid token", ex);
        }
    }

    /// <summary>
    /// Login a user using a google token
    /// </summary>
    /// <param name="googleToken">Token giving by google to connect with their API</param>
    /// <exception cref="InvalidTokenException">Token is invalid</exception>
    /// <exception cref="NotFoundException">User is not found</exception>
    /// <returns>Info of user</returns>
    public async Task<UserInfo> LoginUserAsync(string googleToken)
    {
        try
        {
            GoogleJsonWebSignature.Payload validPayload = null!;

            try
            {
                validPayload = await jwtService.ValidateGoogleTokenAsync(googleToken);
                logger.LogInformation("Token valid");
            }
            catch (Exception)
            {
                throw new InvalidTokenException("Google token is invalid");
            }

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, validPayload.Subject),
                new Claim(JwtRegisteredClaimNames.Email, validPayload.Email),
                new Claim(JwtRegisteredClaimNames.Name, validPayload.Name),
                new Claim("picture", validPayload.Picture ?? ""),
            };

            var user = await userRepository.GetUserByEmailAsync(validPayload.Email);

            if (user is null)
            {
                throw new NotFoundException("User not found");
            }
            else
            {
                try
                {
                    user.LastConnection = DateTime.UtcNow;
                    await userRepository.SaveChangesAsync();

                    logger.LogInformation($"Succes : User {user.Id}'s last connexion updated");
                }
                catch (Exception ex)
                {
                    throw new ContextException("Error while updating the user", ex);
                }
            }

            // Generate and return JWT

            var jwt = jwtService.GenerateToken(claims);

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

            logger.LogInformation("Succes : User logged in and JWT created");

            return userInfo;
        }
        catch (NotFoundException)
        {
            logger.LogError("User not found");
            throw;
        }
        catch (InvalidTokenException)
        {
            logger.LogError("Google token invalid");
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error when saving the info in the DB");
            throw new Exception("Erreur when saving the info in the DB", ex);
        }
    }
}
