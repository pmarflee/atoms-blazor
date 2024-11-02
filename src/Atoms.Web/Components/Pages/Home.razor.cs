using Atoms.Core.Enums;

namespace Atoms.Web.Components.Pages;

public partial class HomePageComponent : ComponentBase
{
    protected GameState State { get; set; }
    protected Game? Game { get; set; }

    protected override void OnInitialized()
    {
        State = GameState.Menu;

        base.OnInitialized();
    }

    protected Task OnCreateGameAsync(Game game)
    {
        Game = game;
        State = GameState.Game;

        return Task.CompletedTask;
    }
}