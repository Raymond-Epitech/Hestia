using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GoogleAuthController(
    IConfiguration configuration,
    IHttpClientFactory httpClientFactory) : Controller
{
    [HttpPost]
    public async Task<ActionResult<string>> AuthentificateWithGoogle([FromBody] GoogleAuthCallbackInput input)
    {
        var clientId = configuration["Google:ClientId"];
        var clientSecret = configuration["Google:ClientSecret"];
        var redirectUri = configuration["Google:RedirectUri"];

        var parameters = new Dictionary<string, string>
        {
            { "code", input.Code },
            { "client_id", clientId },
            { "client_secret", clientSecret },
            { "redirect_uri", redirectUri },
            { "grant_type", "authorization_code" }
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "https://oauth2.googleapis.com/token")
        {
            Content = new FormUrlEncodedContent(parameters)
        };

        var client = httpClientFactory.CreateClient();
        var response = await client.SendAsync(request);
        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            return BadRequest(new { error = "Failed to exchange token", details = responseContent });
        }

        return Ok(responseContent);
    }
}

