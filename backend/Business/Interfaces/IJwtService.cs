using Google.Apis.Auth;
using System.Security.Claims;

namespace Business.Interfaces
{
    public interface IJwtService
    {
        Task<GoogleJsonWebSignature.Payload> ValidateGoogleTokenAsync(string googleToken);
        string GenerateToken(IEnumerable<Claim> claims);
    }
}
