using Atoms.UseCases.GetGame;
using Atoms.UseCases.SetUserName;

namespace Atoms.Web.Components.Pages;

public class GameUserNamePageComponent : Component2Base
{
    [Inject]
    VisitorIdCookieValueService CookieValueService { get; set; } = default!;

    [Inject]
    NavigationManager NavigationManager { get; set; } = default!;

    [CascadingParameter]
    public HttpContext HttpContext { get; set; } = default!;

    [Parameter]
    public Guid GameId { get; set; }

    protected async override Task OnInitializedAsync()
    {
        await LoadGame();
    }

    protected async Task HandleValidUserName(UsernameDTO username)
    {
        var game = await GetGame();

        await Mediator.Send(
            new SetUserNameRequest(
                VisitorId, new UserIdentity(username.Name), game));

        CookieValueService.SetName(HttpContext, username.Name!);

        NavigationManager.NavigateToGame(game);
    }

    async Task LoadGame()
    {
        var game = await GetGame();
        var firstHumanPlayer = game.Players.FirstOrDefault(p => p.IsHuman);

        var canSetUserName = firstHumanPlayer is not null
                             && string.IsNullOrEmpty(firstHumanPlayer.Name);

        if (!canSetUserName)
        {
            NavigationManager.NavigateToGame(game);
        }
    }

    async Task<Game> GetGame()
    {
        var response = await Mediator.Send(
            new GetGameRequest(GameId, VisitorId, UserId));
        var success = response.Success;

        if (!success) NavigationManager.NavigateTo("/");

        return response.Game!;
    }
}
