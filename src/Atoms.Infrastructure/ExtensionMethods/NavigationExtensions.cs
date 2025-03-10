using Flurl;
using Microsoft.AspNetCore.Components;

namespace Atoms.Infrastructure.ExtensionMethods;

public static class NavigationExtensions
{
    public static void NavigateToGame(this NavigationManager navigation,
                                      Game game,
                                      int? debug = null)
    {
        navigation.NavigateToGame(game.Id, debug);
    }

    public static void NavigateToGame(this NavigationManager navigation,
                                      Invite invite)
    {
        navigation.NavigateToGame(invite.GameId);
    }

    static void NavigateToGame(this NavigationManager navigation,
                               Guid gameId,
                               int? debug = null)
    {
        var url = "/games/"
            .AppendPathSegment(gameId)
            .AppendQueryParam("debug", debug);

        navigation.NavigateTo(url);
    }
}
