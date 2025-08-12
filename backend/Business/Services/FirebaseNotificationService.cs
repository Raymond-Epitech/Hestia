using Business.Interfaces;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Options;
using Shared.Models.Configuration;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

public class FirebaseNotificationService : IFirebaseNotificationService
{
    private readonly string _projectId;
    private readonly GoogleCredential _googleCredential;
    private readonly HttpClient _httpClient;
    private readonly FirebaseSettings _settings;

    public FirebaseNotificationService(IOptions<FirebaseSettings> options, HttpClient httpClient)
    {
        _httpClient = httpClient;
        _settings = options.Value;

        _projectId = _settings.ProjectId;

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

    public async Task SendNotificationAsync(List<string> fcmDevices, string title, string body)
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
}