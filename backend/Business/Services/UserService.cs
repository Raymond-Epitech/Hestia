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
    IRepository<FCMDevice> fcmDeviceRepository,
    IJwtService jwtService,
    IFirebaseNotificationService notificationService) : IUserService
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
            .Where(u => u.ColocationId == collocationId && !u.IsDeleted)
            .Select(u => new UserOutput
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                ColocationId = u.ColocationId,
                ProfilePictureUrl = u.PathToProfilePicture
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
                ColocationId = u.ColocationId,
                ProfilePictureUrl = u.PathToProfilePicture
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
        var user = await userRepository.GetByIdAsync(id);

        if (user == null)
            throw new NotFoundException($"User {id} not found");

        user.Email = "";
        user.ColocationId = null;
        user.Username = $"(Deleted) {user.Username}";
        user.PathToProfilePicture = "deleted.jpg";
        user.IsDeleted = true;

        userRepository.Update(user);

        await userRepository.SaveChangesAsync();

        logger.LogInformation("Succes : User deleted");

        return id;
    }

    public async Task<Guid> QuitColocationAsync(Guid id)
    {
        var user = await userRepository.GetByIdAsync(id);

        if (user == null)
            throw new NotFoundException($"User {id} not found");

        user.ColocationId = null;
        userRepository.Update(user);
        await userRepository.SaveChangesAsync();
        logger.LogInformation($"Succes : User {user.Id} quit colocation {user.ColocationId}");
        return user.Id;
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

        logger.LogInformation($"Google link to profil picture is : {validPayload.Picture}");

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
            PathToProfilePicture = validPayload.Picture ?? "default.jpg"
        };

        if (userInput.FCMToken is not null)
        {
            var fmcDevice = new FCMDevice
            {
                FCMToken = userInput.FCMToken,
                UserId = newUser.Id
            };

            await fcmDeviceRepository.AddAsync(fmcDevice);
            
            logger.LogInformation($"Succes : FCM Device {fmcDevice.FCMToken} added for user {newUser.Id}");
        }

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
    public async Task<UserInfo> LoginUserAsync(string googleToken, LoginInput? loginInput)
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

        if (loginInput is not null && await fcmDeviceRepository.Query().AnyAsync(f => f.FCMToken != loginInput.FCMToken))
        {
            var fmcDevice = new FCMDevice
            {
                FCMToken = loginInput.FCMToken,
                UserId = user.Id
            };

            await fcmDeviceRepository.AddAsync(fmcDevice);

            logger.LogInformation($"Succes : FCM Device {fmcDevice.FCMToken} added for user {user.Id}");
        }

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

    /// <summary>
    /// Send a notification to a user
    /// </summary>
    /// <param name="NotificationInput">The id of the user and notification content</param>
    /// <returns>The id of the User</returns>
    /// <exception cref="NotFoundException"></exception>
    public async Task<Guid> SendNotificationToUserAsync(NotificationInput notification)
    {
        var user = await userRepository.GetByIdAsync(notification.Id);

        if (user == null)
            throw new NotFoundException($"User {notification.Id} not found");

        var fcmDevices = await fcmDeviceRepository.Query()
            .Where(f => f.UserId == notification.Id)
            .ToListAsync();

        if (fcmDevices.Count == 0)
            throw new NotFoundException($"No FCM devices found for user {notification.Id}");

        await notificationService.SendNotificationAsync(fcmDevices.Select(f => f.FCMToken).ToList(), notification.Title, notification.Body);

        logger.LogInformation($"Succes : Notification sent to user {notification.Id}");
        
        return notification.Id;
    }

    /// <summary>
    /// Send a notification to all users in a colocation
    /// </summary>
    /// <param name="ColocationId">The id of the colocation and the notification content</param>
    /// <returns>The ids of all the user who received a notification</returns>
    /// <exception cref="NotFoundException"></exception>
    public async Task<List<Guid>> SendNotificationToColocationAsync(NotificationInput notification)
    {
        var users = await userRepository.Query()
            .Where(u => u.ColocationId == notification.Id && !u.IsDeleted)
            .ToListAsync();

        if (users.Count == 0)
            throw new NotFoundException($"No users found in colocation {notification.Id}");

        var fcmDevices = await fcmDeviceRepository.Query()
            .Where(f => users.Select(u => u.Id).Contains(f.UserId))
            .ToListAsync();

        if (fcmDevices.Count == 0)
            throw new NotFoundException($"No FCM devices found for colocation {notification.Id}");

        await notificationService.SendNotificationAsync(fcmDevices.Select(f => f.FCMToken).ToList(), notification.Title, notification.Body);

        logger.LogInformation($"Succes : Notification sent to colocation {notification.Id}");

        return users.Select(u => u.Id).ToList();
    }

    /// <summary>
    /// Delete a fcm device for a user
    /// </summary>
    /// <param name="input">The user id and the fcmToken</param>
    /// <returns>The fcmToken deleted</returns>
    /// <exception cref="NotFoundException"></exception>
    public async Task<string> LogoutUserAsync(LogoutInput input)
    {
        var fcmDevice = await fcmDeviceRepository.Query()
            .Where(f => f.UserId == input.UserId && f.FCMToken == input.FCMToken)
            .FirstOrDefaultAsync();

        if (fcmDevice == null)
            throw new NotFoundException($"FcmToken {input.FCMToken} not found");

        fcmDeviceRepository.Delete(fcmDevice);

        logger.LogInformation($"Succes : FCM Device {input.FCMToken} deleted for user {input.UserId}");
        return fcmDevice.FCMToken;
    }
}
