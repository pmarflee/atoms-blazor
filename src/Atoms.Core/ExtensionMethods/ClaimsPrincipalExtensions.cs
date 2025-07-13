using System.Security.Claims;

namespace Atoms.Core.ExtensionMethods;

public static class ClaimsPrincipalExtensions
{
    public static UserId? GetUserId(this ClaimsPrincipal? claimsPrincipal)
    {
        return claimsPrincipal?.Identity?.IsAuthenticated == true
            ? claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value
            : null;
    }

    public static string? GetUserName(this ClaimsPrincipal? claimsPrincipal)
        => claimsPrincipal?.Identity?.Name;
}
