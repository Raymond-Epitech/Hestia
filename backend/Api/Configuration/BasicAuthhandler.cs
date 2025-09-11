// BasicAuthHandler.cs
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;

public class BasicAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public BasicAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
                            ILoggerFactory logger, UrlEncoder encoder, TimeProvider clock)
        : base(options, logger, encoder) { }

    protected override Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Response.Headers["WWW-Authenticate"] = "Basic realm=\"Restricted\", charset=\"UTF-8\"";
        return base.HandleChallengeAsync(properties);
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("Authorization", out var header))
            return Task.FromResult(AuthenticateResult.Fail("Missing Authorization Header"));

        if (!AuthenticationHeaderValue.TryParse(header, out var auth) || !"Basic".Equals(auth.Scheme, StringComparison.OrdinalIgnoreCase))
            return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization Header"));

        var credsBytes = Convert.FromBase64String(auth.Parameter ?? "");
        var creds = Encoding.UTF8.GetString(credsBytes).Split(':', 2);
        if (creds.Length != 2) return Task.FromResult(AuthenticateResult.Fail("Invalid Basic Credentials"));

        var user = creds[0];
        var pass = creds[1];

        var cfgUser = Context.RequestServices.GetRequiredService<IConfiguration>()["BasicAuth:Username"] ?? "";
        var cfgPass = Context.RequestServices.GetRequiredService<IConfiguration>()["BasicAuth:Password"] ?? "";

        bool okUser = CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(user), Encoding.UTF8.GetBytes(cfgUser));
        bool okPass = CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(pass), Encoding.UTF8.GetBytes(cfgPass));
        if (!(okUser && okPass)) return Task.FromResult(AuthenticateResult.Fail("Invalid username or password"));

        var claims = new[] { new Claim(ClaimTypes.Name, user), new Claim(ClaimTypes.Role, "Admin") };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
