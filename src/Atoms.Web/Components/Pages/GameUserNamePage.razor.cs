using Atoms.UseCases.GetGame;

namespace Atoms.Web.Components.Pages;

public class GameUserNamePageComponent : Component2Base
{
    [Inject]
    NavigationManager NavigationManager { get; set; } = default!;

    [Parameter]
    public Guid GameId { get; set; }

    protected async override Task OnInitializedAsync()
    {
        await LoadGame();
    }

    async Task LoadGame()
    {
        var response = await Mediator.Send(
            new GetGameRequest(GameId, VisitorId, UserId));
        var success = response.Success;

        if (!success) NavigationManager.NavigateTo("/");

        var game = response.Game!;

        var firstHumanPlayer = game.Players.FirstOrDefault(p => p.IsHuman);

        success = firstHumanPlayer is not null
                  && string.IsNullOrEmpty(firstHumanPlayer.Name);

        if (!success)
        {
            NavigationManager.NavigateToGame(game);
        }
    }
}
