using Business.Interfaces;
using EntityFramework.Models;
using EntityFramework.Repositories;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Exceptions;
using Shared.Models.Configuration;
using Shared.Models.Input;
using System.Text.Json;

public class FirebaseNotificationService : IFirebaseNotificationService
{
    private readonly string _projectId;
    private readonly GoogleCredential _googleCredential;
    private readonly HttpClient _httpClient;
    private readonly FirebaseSettings _settings;
    private readonly IRepository<User> userRepository;
    private readonly IRepository<FCMDevice> fcmdeviceRepository;
    private readonly ILogger<FirebaseNotificationService> logger;

    public FirebaseNotificationService(
        IOptions<FirebaseSettings> options,
        HttpClient httpClient,
        IRepository<User> userRepository,
        IRepository<FCMDevice> fcmDeviceRepository,
        ILogger<FirebaseNotificationService> logger)
    {
        _httpClient = httpClient;
        _settings = options.Value;
        _projectId = _settings.ProjectId;
        this.userRepository = userRepository;
        this.fcmdeviceRepository = fcmDeviceRepository;
        this.logger = logger;

        _settings.PrivateKey = _settings.PrivateKey.Replace("\\n", "\n");
        var credentialJson = JsonSerializer.Serialize(new
        {
            type = "service_account",
            project_id = _projectId,
            private_key_id = _settings.PrivateKeyId,
            private_key = _settings.PrivateKey,
            client_email = _settings.ClientEmail,
            client_id = _settings.ClientId,
        });

        _googleCredential = GoogleCredential.FromJson(credentialJson)
            .CreateScoped("https://www.googleapis.com/auth/firebase.messaging");
    }

    private static IEnumerable<List<string>> Chunk(List<string> source, int size)
    {
        for (int i = 0; i < source.Count; i += size)
            yield return source.GetRange(i, Math.Min(size, source.Count - i));
    }

    private async Task SendNotificationAsync(List<string> fcmTokens, string title, string body, CancellationToken ct = default)
    {
        if (fcmTokens is null || fcmTokens.Count == 0) return;

        int totalSuccess = 0;
        int totalFailure = 0;

        foreach (var tokenBatch in Chunk(fcmTokens, 500))
        {
            var message = new MulticastMessage
            {
                Tokens = tokenBatch,
                Notification = new Notification { Title = title, Body = body },
            };

            BatchResponse batchResponse;
            try
            {
                batchResponse = await FirebaseMessaging.DefaultInstance.SendEachForMulticastAsync(message, ct);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"FCM multicast call failed for a chunk of {tokenBatch.Count} tokens");
                continue;
            }

            totalSuccess += batchResponse.SuccessCount;
            totalFailure += batchResponse.FailureCount;

            for (int i = 0; i < batchResponse.Responses.Count; i++)
            {
                var responce = batchResponse.Responses[i];
                var token = tokenBatch[i];

                if (responce.IsSuccess)
                    continue;

                var fcmExpcetion = responce.Exception as FirebaseMessagingException;

                logger.LogWarning(responce.Exception, $"FCM failed for token {token}. Code={fcmExpcetion?.MessagingErrorCode} Msg={responce.Exception.Message}");

                if (fcmExpcetion?.MessagingErrorCode == MessagingErrorCode.Unregistered ||
                    fcmExpcetion?.MessagingErrorCode == MessagingErrorCode.InvalidArgument)
                {
                    await fcmdeviceRepository.Query()
                        .Where(d => d.FCMToken == token)
                        .ExecuteDeleteAsync(ct);
                }
                else if (fcmExpcetion?.MessagingErrorCode == MessagingErrorCode.Internal ||
                         fcmExpcetion?.MessagingErrorCode == MessagingErrorCode.Unavailable)
                {
                    logger.LogInformation($"Transient FCM error for token {token} - consider retrying later");
                }
            }
        }

        logger.LogInformation($"FCM multicast done. Success={totalSuccess} Failure={totalFailure}");
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

        var fcmDevices = user.FCMDevices;

        if (fcmDevices.Count == 0)
        {
            logger.LogInformation($"No fcm device for user {user.Id}");
            return Guid.Empty;
        }

        await SendNotificationAsync(fcmDevices.Select(f => f.FCMToken).ToList(), notification.Title, notification.Body);

        logger.LogInformation($"Succes : Notification sent to user {notification.Id}");

        return notification.Id;
    }

    /// <summary>
    /// Send a notification to all users in a colocation
    /// </summary>
    /// <param name="ColocationId">The id of the colocation and the notification content</param>
    /// <param name="UserId">The user to exclude from the notification</param>
    /// <returns>The ids of all the user who received a notification</returns>
    /// <exception cref="NotFoundException"></exception>
    public async Task<List<Guid>> SendNotificationToColocationAsync(NotificationInput notification, Guid? UserId)
    {
        var users = await userRepository.Query()
            .Where(u => u.ColocationId == notification.Id && !u.IsDeleted)
            .Include(u => u.FCMDevices)
            .ToListAsync();

        if (users.Count == 0)
        {
            logger.LogInformation($"No users found for colocation {notification.Id}");
            return new List<Guid>();
        }

        if (UserId is not null)
        {
            var user = users.FirstOrDefault(u => u.Id == UserId);
            if (user != null)
            {
                users.Remove(user);
            }
        }

        logger.LogInformation($"Found {users.Count} users for colocation {notification.Id}");

        foreach (var u in users)
        {
            logger.LogInformation($"User {u.Id} has {u.FCMDevices?.Count ?? 0} fcm devices");
        }

        var fcmDevices = users
            .Where(u => u.FCMDevices != null)
            .SelectMany(u => u.FCMDevices!)
            .ToList();

        if (fcmDevices.Count == 0)
        {
            logger.LogInformation($"No user found");
            return new List<Guid>();
        }

        logger.LogInformation($"Found {fcmDevices.Count} fcm devices for colocation {notification.Id}");

        await SendNotificationAsync(fcmDevices.Select(f => f.FCMToken).ToList(), notification.Title, notification.Body);

        logger.LogInformation($"Succes : Notification sent to colocation {notification.Id}");

        return users.Select(u => u.Id).ToList();
    }
}