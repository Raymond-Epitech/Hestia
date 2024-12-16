using Business.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace Business.Services
{
    public class UserService : IUserService
    {
        public bool LoginUser(string googleToken, string clientId)
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = "https://accounts.google.com",
                ValidateAudience = true,
                ValidAudience = clientId,
                ValidateLifetime = true,
                IssuerSigningKeyResolver = (token, securityToken, kid, parameters) =>
                {
                    // Récupérer les clés publiques depuis Google
                    var client = new HttpClient();
                    var keys = client.GetStringAsync("https://www.googleapis.com/oauth2/v3/certs").Result;
                    return new JsonWebKeySet(keys).Keys;
                }
            };

            try
            {
                var handler = new JwtSecurityTokenHandler();
                handler.ValidateToken(googleToken, validationParameters, out _);
                return true; // Le token est valide
            }
            catch
            {
                return false; // Token invalide
            }
        }
    }
}
