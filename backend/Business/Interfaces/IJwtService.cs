using System.Security.Claims;

namespace Business.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(IEnumerable<Claim> claims);
    }
}
