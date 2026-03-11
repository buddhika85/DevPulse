using System.Security.Claims;

namespace SharedLib.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        // returns Azure AD / Microsoft Entra IDs - Object ID / oid from JWT
        public static string? GetOid(this ClaimsPrincipal user)
        {
            return user.FindFirst("oid")?.Value ??
                   user.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;
        }
    }

}
