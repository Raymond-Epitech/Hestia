using Business.Interfaces;
using Business.Jwt;
using EntityFramework.Models;
using EntityFramework.Repositories;
using Google.Apis.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Exceptions;
using Shared.Models.Input;
using Shared.Models.Output;
using Shared.Models.Update;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Business.Services;

public class UserService(ILogger<UserService> logger,
    IRepository<User> userRepository,
    IJwtService jwtService) : IUserService
{
    /// <summary>
    /// Get all users from a collocation
    /// </summary>
    /// <param name="CollocationId">The Id of the collocation you want the users from</param>
    /// <returns>The list of the User in a collocation</returns>
    /// <exception cref="ContextException">An error occurred while getting all chores from the db</exception>
    public async Task<List<UserOutput>> GetAllUserAsync(Guid collocationId)
    {
        var users = await userRepository.Query()
            .Where(u => u.ColocationId == collocationId)
            .Select(u => new UserOutput
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                ColocationId = u.ColocationId
            })
            .ToListAsync();

        logger.LogInformation($"Succes : All users from the collocation {collocationId} found");

        return users;
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
        var user = await userRepository.Query()
            .Where(u => u.Id == id)
            .Select(u => new UserOutput
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                ColocationId = u.ColocationId
            })
            .FirstOrDefaultAsync();

        if (user == null)
            throw new NotFoundException("User not found");

        logger.LogInformation($"Succes : User {id} found");

        return user;
    }

    /// <summary>
    /// Update a user
    /// </summary>
    /// <param name="user">The user to update with new info</param>
    /// <exception cref="NotFoundException">The user was not found</exception>
    /// <exception cref="ContextException">An error occurred while getting all chores from the db</exception>
    public async Task<Guid> UpdateUserAsync(UserUpdate user)
    {
        var userToUpdate = await userRepository.GetByIdAsync(user.Id);

        if (userToUpdate == null)
            throw new NotFoundException($"User {user.Id} not found");

        userToUpdate.Username = user.Username;
        userToUpdate.ColocationId = user.ColocationId;
        userToUpdate.PathToProfilePicture = user.PathToProfilePicture;

        await userRepository.SaveChangesAsync();

        logger.LogInformation($"Succes : User {user.Id} updated");
        
        return userToUpdate.Id;
    }

    /// <summary>
    /// To delete a user
    /// </summary>
    /// <param name="id">The id of the user you want to delete</param>
    /// <exception cref="NotFoundException">The user was not found</exception>
    /// <exception cref="ContextException">An error occurred while getting all chores from the db</exception>
    public async Task<Guid> DeleteUserAsync(Guid id)
    {
        await userRepository.DeleteFromIdAsync(id);

        await userRepository.SaveChangesAsync();

        logger.LogInformation("Succes : User deleted");

        return id;
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
        GoogleJsonWebSignature.Payload validPayload = null!;

        validPayload = new GoogleJsonWebSignature.Payload()
        {
            Email = "test@gmail.com"
        };
                
        try
        {
            validPayload = await jwtService.ValidateGoogleTokenAsync(googleToken);
            logger.LogInformation("Token valid");
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Google token validation failed.");
            throw new InvalidTokenException("Google token invalid");
        }
            
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, validPayload.Subject),
            new Claim(JwtRegisteredClaimNames.Email, validPayload.Email),
            new Claim(JwtRegisteredClaimNames.Name, validPayload.Name),
            new Claim("picture", validPayload.Picture ?? ""),
        };

        if (userRepository.Query().Any(u => u.Email == validPayload.Email))
        {
            throw new AlreadyExistException("This user already exist with this email");
        }

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

        await userRepository.AddAsync(newUser);

        await userRepository.SaveChangesAsync();

        logger.LogInformation($"Succes : User {newUser.Id} added");

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

    /// <summary>
    /// Login a user using a google token
    /// </summary>
    /// <param name="googleToken">Token giving by google to connect with their API</param>
    /// <exception cref="InvalidTokenException">Token is invalid</exception>
    /// <exception cref="NotFoundException">User is not found</exception>
    /// <returns>Info of user</returns>
    public async Task<UserInfo> LoginUserAsync(string googleToken)
    {
        GoogleJsonWebSignature.Payload validPayload = null!;

        try
        {
            validPayload = await jwtService.ValidateGoogleTokenAsync(googleToken);
            logger.LogInformation("Token valid");
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Google token validation failed.");
            throw new InvalidTokenException("Google token invalid");
        }

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, validPayload.Subject),
            new Claim(JwtRegisteredClaimNames.Email, validPayload.Email),
            new Claim(JwtRegisteredClaimNames.Name, validPayload.Name),
            new Claim("picture", validPayload.Picture ?? ""),
        };

        var user = await userRepository.Query()
            .Where(u => u.Email == validPayload.Email)
            .FirstOrDefaultAsync();

        if (user is null)
        {
            throw new NotFoundException("User not found");
        }

        user.LastConnection = DateTime.UtcNow;

        userRepository.Update(user);
        await userRepository.SaveChangesAsync();

        logger.LogInformation($"Succes : User {user.Id}'s last connexion updated");

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
}
