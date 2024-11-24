using Atoms.UseCases.CreateNewGame;

namespace Atoms.Web.Components.Pages;

public partial class HomePageComponent : Component2Base
{
    [Inject]
    IJSRuntime JSRuntime { get; set; } = default!;

    [SupplyParameterFromQuery]
    protected int? Debug { get; set; }

    protected GameState State { get; set; }
    protected Game? Game { get; set; }

    protected async override Task OnInitializedAsync()
    {
        if (Debug.HasValue)
        {
            var request = new CreateNewGameRequest(GameMenuOptions.Debug);
            var response = await Mediator.Send(request);

            await StartGame(response.Game);
        }
        else
        {
            State = GameState.Menu;
        }
    }

    protected async Task OnCreateGame(Game game)
    {
        await StartGame(game);
    }

    protected Task OnShowMenu()
    {
        Game = null;
        State = GameState.Menu;

        return Task.CompletedTask;
    }

    async Task StartGame(Game game)
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
}