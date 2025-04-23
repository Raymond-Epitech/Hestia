using Business.Interfaces;
using Business.Jwt;
using EntityFramework.Models;
using EntityFramework.Repositories;
using Google.Apis.Auth;
using Google.Apis.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Exceptions;
using Shared.Models.DTO;
using Shared.Models.Input;
using Shared.Models.Output;
using Shared.Models.Update;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

namespace Business.Services;

public class UserService(ILogger<UserService> logger,
    IRepository<User> userRepository,
    IJwtService jwtService,
    System.Net.Http.IHttpClientFactory httpClientFactory) : IUserService
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

    public async Task<Guid> QuitColocationAsync(Guid id)
    {
        var user = await userRepository.GetByIdAsync(id);

        if (user == null)
            throw new NotFoundException($"User {id} not found");

        user.ColocationId = Guid.Empty;
        userRepository.Update(user);
        await userRepository.SaveChangesAsync();
        logger.LogInformation($"Succes : User {user.Id} quit colocation {user.ColocationId}");
        return user.Id;
    }

    private async Task<string> GetGoogleJwt(GoogleCredentials googleCredentials, string code)
    {

        if (string.IsNullOrEmpty(googleCredentials.ClientId) ||
            string.IsNullOrEmpty(googleCredentials.ClientSecret) ||
            string.IsNullOrEmpty(googleCredentials.RedirectUri))
        {
            throw new InvalidEntityException("Google credentials are not set");
        }

        var parameters = new Dictionary<string, string>
        {
            { "code", code },
            { "client_id", googleCredentials.ClientId },
            { "client_secret", googleCredentials.ClientSecret },
            { "redirect_uri", googleCredentials.RedirectUri },
            { "grant_type", "authorization_code" }
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "https://oauth2.googleapis.com/token")
        {
            Content = new FormUrlEncodedContent(parameters)
        };

        var client = httpClientFactory.CreateClient();
        var response = await client.SendAsync(request);
        var responseContent = await response.Content.ReadAsStringAsync();
        var tokenResponse = JsonSerializer.Deserialize<GoogleTokenResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (!response.IsSuccessStatusCode || tokenResponse is null || tokenResponse.IdToken is null)
        {
            throw new InvalidEntityException($"Google token exchange failed: {responseContent}");
        }
        
        return tokenResponse.IdToken;
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    /// <param name="googleToken">the token from the google API OAuth 2</param>
    /// <param name="userInput">The info for the new user</param>
    /// <returns>The JWT and the new user's info</returns>
    /// <exception cref="ContextException">Error in the DB or context</exception>
    /// <exception cref="AlreadyExistException">User already registered</exception>
    public async Task<UserInfo> RegisterUserAsync(string code, UserInput userInput, GoogleCredentials googleCredentials)
    {
        var googleToken = await GetGoogleJwt(googleCredentials, code);

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
    public async Task<UserInfo> LoginUserAsync(string code, GoogleCredentials googleCredentials)
    {
        var googleToken = await GetGoogleJwt(googleCredentials, code);

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
