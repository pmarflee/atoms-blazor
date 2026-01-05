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

        public void NavigateToSetUserNamePage(Game game)
        {
            var url = BaseUrl.AppendPathSegment(game.Id)
                             .AppendPathSegment("/username/");

            navigation.NavigateTo(url);
        }

        public string GetAbsoluteGameUrl(Guid gameId)
        {
            var url = navigation.GetGameUrl(gameId);

            return navigation.ToAbsoluteUri(url).AbsoluteUri;
        }

        void NavigateToGameById(Guid gameId, int? debug = null)
        {
            var url = navigation.GetGameUrl(gameId, debug);

            navigation.NavigateTo(url, true);
        }

        string GetGameUrl(Guid gameId, int? debug = null)
        {
            var url = BaseUrl.AppendPathSegment(gameId.ToString("N"));

            if (debug.HasValue)
            {
                url = url.AppendQueryParam("debug", debug);
            }

            return url;
        }
    }
}
