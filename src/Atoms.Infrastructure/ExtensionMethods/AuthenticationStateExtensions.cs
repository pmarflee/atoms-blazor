using Microsoft.AspNetCore.Components.Authorization;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace Atoms.Infrastructure.ExtensionMethods;

public static class AuthenticationStateExtensions
{
    public static bool TryGetUserId(this AuthenticationState authenticationState, [MaybeNullWhen(false)] out UserId? userId)
    {
        var user = authenticationState.User;
        var userIdString = user.Identity?.IsAuthenticated ?? false
            ? user.FindFirst(ClaimTypes.NameIdentifier)?.Value
            : null;

        if (userIdString is null)
        {
            userId = null;

            return false;
        }

        userId = new(userIdString);

        return true;
    }
}
