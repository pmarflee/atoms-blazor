using Microsoft.JSInterop;

namespace Atoms.Web.Components.Pages;

public partial class HomePageComponent : ComponentBase
{
    [Inject]
    IJSRuntime JSRuntime { get; set; } = default!;

    protected GameState State { get; set; }
    protected Game? Game { get; set; }

    protected override void OnInitialized()
    {
        State = GameState.Menu;

        base.OnInitialized();
    }

    protected async Task OnCreateGame(Game game)
    {
        Game = game;
        State = GameState.Game;

        if (Game.ColourScheme == ColourScheme.Alternate)
        {
            await JSRuntime.InvokeVoidAsync("App.setAlternateColourScheme");
        }
        else
        {
            await JSRuntime.InvokeVoidAsync("App.setDefaultColourScheme");
        }

        if (Game.AtomShape == AtomShape.Varied)
        {
            await JSRuntime.InvokeVoidAsync("App.setVariedAtomShape");
        }
        else
        {
            await JSRuntime.InvokeVoidAsync("App.setDefaultAtomShape");
        }
    }

    protected Task OnShowMenu()
    {
        Game = null;
        State = GameState.Menu;

        return Task.CompletedTask;
    }
}