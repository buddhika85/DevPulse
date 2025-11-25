using System.Security.Claims;

namespace UserService.Services
{
    public interface ITokenService
    {
        Task<ClaimsPrincipal?> ValidateEntraTokenAsync(string entraToken, CancellationToken cancellationToken);
        string GenerateAppToken(ClaimsPrincipal entraClaims, string role, CancellationToken cancellationToken);
    }
}
