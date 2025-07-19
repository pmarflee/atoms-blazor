using Atoms.UseCases.GetGame;
using Atoms.UseCases.SetUserName;

namespace Atoms.Web.Components.Pages;

public class UserNamePageComponent : Component2Base
{
    [Inject]
    NavigationManager Navigation { get; set; } = default!;

    [Parameter]
    public Guid GameId { get; set; }

    protected string? Name { get; set; }

    Game _game = default!;

    protected async override Task OnInitializedAsync()
    {
        await LoadGame();

        Name = await GetUserName();
    }

    async Task LoadGame()
    {
        var storageId = await GetOrAddStorageId();
        var response = await Mediator.Send(
            new GetGameRequest(GameId, storageId, UserId));
        var success = response.Success;

        if (!success) Navigation.NavigateTo("/");

        _game = response.Game!;

        var firstHumanPlayer = _game.Players.FirstOrDefault(p => p.IsHuman);

        success = firstHumanPlayer is not null
                  && string.IsNullOrEmpty(firstHumanPlayer.Name);

        if (!success) Navigation.NavigateToGame(_game);
    }

    protected async Task NameUpdated()
    {
        await Mediator.Send(new SetUserNameRequest(_game, new(Name)));

        Navigation.NavigateToGame(_game);
    }
}
