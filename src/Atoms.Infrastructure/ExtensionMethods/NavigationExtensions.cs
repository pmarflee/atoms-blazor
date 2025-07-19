using Flurl;
using Microsoft.AspNetCore.Components;

namespace Atoms.Infrastructure.ExtensionMethods;

public static class NavigationExtensions
{
    const string BaseUrl = "/games/";

    public static void NavigateToGame(this NavigationManager navigation,
                                      Game game)
    {
        navigation.NavigateToGame(game.Id);
    }

    public static void NavigateToGame(this NavigationManager navigation,
                                      Invite invite)
    {
        navigation.NavigateToGame(invite.GameId);
    }

    public static void NavigateToDebugGame(this NavigationManager navigation,
                                           int debug)
    {
        navigation.NavigateToGame(debug: debug);
    }

    public static void NavigateToSetUserNamePage(
        this NavigationManager navigation, Game game)
    {
        var url = BaseUrl.AppendPathSegment(game.Id)
                         .AppendPathSegment("/username/");

        navigation.NavigateTo(url);
    }

    static void NavigateToGame(this NavigationManager navigation,
                               Guid? gameId = null,
                               int? debug = null)
    {
        var url = BaseUrl;

        if (gameId.HasValue)
        {
            url = url.AppendPathSegment(gameId);
        }

        if (debug.HasValue)
        {
            url = url.AppendQueryParam("debug", debug);
        }

        navigation.NavigateTo(url);
    }
}
