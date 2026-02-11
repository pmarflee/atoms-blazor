using Flurl;
using Microsoft.AspNetCore.Components;

namespace Atoms.Infrastructure.ExtensionMethods;

public static class NavigationExtensions
{
    const string BaseUrl = "/games/";

    extension(NavigationManager navigation)
    {
        public void NavigateToGame(Game game)
        {
            navigation.NavigateToGameById(game.Id);
        }

        public void NavigateToGame(GameDTO gameDto)
        {
            navigation.NavigateToGameById(gameDto.Id);
        }

        public void NavigateToGame(GameInfoDTO game)
        {
            navigation.NavigateToGameById(game.Id);
        }

        public void NavigateToGame(Guid gameId)
        {
            navigation.NavigateToGameById(gameId);
        }

        public void NavigateToDebugGame(int debug)
        {
            navigation.NavigateToGameById(Guid.NewGuid(), debug);
        }

        public void NavigateToSetUserNamePage(GameDTO gameDto)
        {
            navigation.NavigateToSetUserNamePage(gameDto.Id);
        }

        public void NavigateToSetUserNamePage(Game game)
        {
            navigation.NavigateToSetUserNamePage(game.Id);
        }

        void NavigateToSetUserNamePage(Guid gameId)
        {
            var url = BaseUrl.AppendPathSegment(gameId)
                             .AppendPathSegment("/username/");

            navigation.NavigateTo(url, true);
        }

        public string GetAbsoluteGameUrl(Guid gameId)
        {
            var url = GetGameUrl(gameId);

            return navigation.ToAbsoluteUri(url).AbsoluteUri;
        }

        void NavigateToGameById(Guid gameId, int? debug = null)
        {
            var url = GetGameUrl(gameId, debug);

            navigation.NavigateTo(url, true);
        }
    }

    static string GetGameUrl(Guid gameId, int? debug = null)
    {
        var url = BaseUrl.AppendPathSegment(gameId.ToString("N"));

        if (debug.HasValue)
        {
            url = url.AppendQueryParam("debug", debug);
        }

        return url;
    }
}
