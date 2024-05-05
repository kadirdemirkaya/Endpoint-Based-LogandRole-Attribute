using EndpointBasedRole.Models;

namespace EndpointBasedRole.Abstractions
{
    public interface ITokenService
    {
        Token GenerateToken(string email, string id, string? role = null);
    }
}
