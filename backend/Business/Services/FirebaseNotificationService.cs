using Business.Interfaces;
using Microsoft.Extensions.Options;
using Shared.Models.Configuration;
using System.Text;
using System.Text.Json;

public class FirebaseNotificationService : IFirebaseNotificationService
{
    private readonly HttpClient _httpClient;
    private readonly FirebaseSettings _settings;
    private const string FirebaseUrl = "https://fcm.googleapis.com/fcm/send";

    public FirebaseNotificationService(HttpClient httpClient, IOptions<FirebaseSettings> options)
    {
        _httpClient = httpClient;
        _settings = options.Value;
    }

    public async Task SendNotificationAsync(string title, string body)
    {
        foreach (var token in _settings.Tokens)
        {
            var message = new
            {
                to = token,
                notification = new
                {
                    title,
                    body
                }
            };

            var request = new HttpRequestMessage(HttpMethod.Post, FirebaseUrl);
            request.Headers.TryAddWithoutValidation("Authorization", $"key={_settings.ServerKey}");
            request.Content = new StringContent(JsonSerializer.Serialize(message), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }
    }
}