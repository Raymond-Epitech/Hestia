using Business.Interfaces;
using Microsoft.Extensions.Configuration;
using Shared.Models.DTO;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Business.Services;

public class ReceiptScannerService(IHttpClientFactory httpClientFactory, IConfiguration configuration) : IReceiptScannerService
{
    private readonly string _apiKey = configuration["GoogleAI:ApiKey"]
                                      ?? throw new Exception("Clé GoogleAI:ApiKey introuvable !");
    private readonly string _aiVersion = configuration["GoogleAI:AiModel"]
                                      ?? throw new Exception("Clé GoogleAI:AiModel introuvable !");

    public async Task<ReceiptScanResult> ScanReceiptAsync(Stream imageStream, string contentType)
    {
        // 1. Conversion image
        using var memoryStream = new MemoryStream();
        await imageStream.CopyToAsync(memoryStream);
        string base64Image = Convert.ToBase64String(memoryStream.ToArray());

        // 2. Construction du Payload
        var payload = new
        {
            contents = new[]
            {
                new
                {
                    parts = new object[]
                    {
                        new { text = GetSystemPrompt() },
                        new
                        {
                            inline_data = new
                            {
                                mime_type = contentType,
                                data = base64Image
                            }
                        }
                    }
                }
            },
            generationConfig = new
            {
                response_mime_type = "application/json"
            }
        };

        // 3. Création du client et de l'URL
        var client = httpClientFactory.CreateClient();

        string url = $"https://generativelanguage.googleapis.com/v1beta/models/{_aiVersion}:generateContent?key={_apiKey}";

        Console.WriteLine($"DEBUG: Calling URL -> {url.Replace(_apiKey, "HIDDEN_KEY")}");

        // 4. Envoi
        var response = await client.PostAsJsonAsync(url, payload);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Google API Error {response.StatusCode}. Details: {errorContent}");
        }

        // 5. Parsing
        var googleResponse = await response.Content.ReadFromJsonAsync<GeminiResponse>();
        var rawJsonText = googleResponse?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;

        if (string.IsNullOrEmpty(rawJsonText)) return new ReceiptScanResult();

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        return JsonSerializer.Deserialize<ReceiptScanResult>(rawJsonText, options) ?? new ReceiptScanResult();
    }

    private string GetSystemPrompt()
    {
        return """
               Extract data from this receipt image.
               Return ONLY valid JSON matching this structure exactly:
               {
                 "TotalAmount": 0.00,
                 "DateOfPurchase": "YYYY-MM-DD",
                 "Items": [
                   { "Name": "Item Name", "Price": 0.00 }
                 ]
               }
               Rules:
               - DateOfPurchase format YYYY-MM-DD or null.
               - Do not aggregate duplicate items.
               - Exclude tax/subtotal lines.
               """;
    }

    private class GeminiResponse
    {
        [JsonPropertyName("candidates")] public List<Candidate>? Candidates { get; set; }
    }
    private class Candidate
    {
        [JsonPropertyName("content")] public Content? Content { get; set; }
    }
    private class Content
    {
        [JsonPropertyName("parts")] public List<Part>? Parts { get; set; }
    }
    private class Part
    {
        [JsonPropertyName("text")] public string? Text { get; set; }
    }
}
