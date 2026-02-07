using Atoms.UseCases.GetGame;
using Atoms.UseCases.SetUserName;

namespace Atoms.Web.Components.Pages;

public class GameUserNamePageComponent : Component2Base
{
    [Inject]
    NavigationManager Navigation { get; set; } = default!;

    [Parameter]
    public Guid GameId { get; set; }

    Game _game = default!;

    protected async override Task OnInitializedAsync()
    {
        await LoadGame();
    }

    async Task LoadGame()
    {
        var response = await Mediator.Send(
            new GetGameRequest(GameId, VisitorId, UserId));
        var success = response.Success;

        if (!success) Navigation.NavigateTo("/");

        _game = response.Game!;

        var firstHumanPlayer = _game.Players.FirstOrDefault(p => p.IsHuman);

        success = firstHumanPlayer is not null
                  && string.IsNullOrEmpty(firstHumanPlayer.Name);

        if (!success) Navigation.NavigateToGame(_game);
    }

    protected async Task NameChanged(string name)
    {
        await Mediator.Send(new SetUserNameRequest(VisitorId, new(name), _game));

        Navigation.NavigateToGame(_game);
    }
}
