using Business.Interfaces;
using EntityFramework.Models;
using EntityFramework.Repositories;
using Google.Apis.Auth.OAuth2;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Exceptions;
using Shared.Models.Configuration;
using Shared.Models.Input;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

public class FirebaseNotificationService : IFirebaseNotificationService
{
    private readonly string _projectId;
    private readonly GoogleCredential _googleCredential;
    private readonly HttpClient _httpClient;
    private readonly FirebaseSettings _settings;
    private readonly IRepository<User> userRepository;
    private readonly ILogger<FirebaseNotificationService> logger;

    public FirebaseNotificationService(
        IOptions<FirebaseSettings> options,
        HttpClient httpClient,
        IRepository<User> userRepository,
        ILogger<FirebaseNotificationService> logger)
    {
        _httpClient = httpClient;
        _settings = options.Value;
        _projectId = _settings.ProjectId;
        this.userRepository = userRepository;
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

    private async Task SendNotificationAsync(List<string> fcmDevices, string title, string body)
    {
        var accessToken = await _googleCredential.UnderlyingCredential
            .GetAccessTokenForRequestAsync();

        foreach (var token in fcmDevices)
        {
            var messagePayload = new
            {
                message = new
                {
                    token = token,
                    notification = new
                    {
                        title,
                        body
                    }
                }
            };

            var json = JsonSerializer.Serialize(messagePayload);
            var request = new HttpRequestMessage(
                HttpMethod.Post,
                $"https://fcm.googleapis.com/v1/projects/{_projectId}/messages:send"
            );
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Erreur FCM : {response.StatusCode} - {error}");
            }
        }
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

        await SendNotificationAsync(fcmDevices.Select(f => f.FCMToken).ToList(), notification.Title, notification.Body);

        logger.LogInformation($"Succes : Notification sent to colocation {notification.Id}");

        return users.Select(u => u.Id).ToList();
    }
}