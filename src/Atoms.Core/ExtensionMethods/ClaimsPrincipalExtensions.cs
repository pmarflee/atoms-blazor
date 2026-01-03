using System.Security.Claims;

namespace Atoms.Core.ExtensionMethods;

public static class ClaimsPrincipalExtensions
{
    extension(ClaimsPrincipal? claimsPrincipal)
    {
        public UserId? GetUserId()
        {
            return claimsPrincipal?.Identity?.IsAuthenticated == true
                ? claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value
                : null;
        }

        public string? GetUserName() => claimsPrincipal?.Identity?.Name;
    }
}
