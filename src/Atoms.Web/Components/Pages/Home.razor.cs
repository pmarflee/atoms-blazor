using Atoms.UseCases.CreateNewGame;

namespace Atoms.Web.Components.Pages;

public partial class HomePageComponent : Component2Base, IDisposable
{
    [Inject]
    IJSRuntime JSRuntime { get; set; } = default!;

    [Inject]
    GameStateContainer StateContainer { get; set; } = default!;

    [Inject]
    ProtectedLocalStorage ProtectedLocalStore { get; set; } = default!;

    [SupplyParameterFromQuery]
    protected int? Debug { get; set; }

    protected async override Task OnInitializedAsync()
    {
        await InitializeLocalStorageId();

        StateContainer.OnChange += StateHasChangedAsync;

        if (Debug.HasValue)
        {
            var request = new CreateNewGameRequest(GameMenuOptions.Debug);
            var response = await Mediator.Send(request);

            await StartGame(response.Game);
        }
        else
        {
            await StateContainer.SetMenu();
        }
    }

    protected async Task OnCreateGame(Game game)
    {
        await StartGame(game);
    }

    protected async Task OnShowMenu()
    {
        await StateContainer.SetMenu();
    }

    protected GameState State => StateContainer.State;

    async Task StartGame(Game game)
    {
        await StateContainer.SetGame(game);

        if (game.ColourScheme == ColourScheme.Alternate)
        {
            await JSRuntime.InvokeVoidAsync("App.setAlternateColourScheme");
        }
        else
        {
            await JSRuntime.InvokeVoidAsync("App.setDefaultColourScheme");
        }

        if (game.AtomShape == AtomShape.Varied)
        {
            await JSRuntime.InvokeVoidAsync("App.setVariedAtomShape");
        }
        else
        {
            await JSRuntime.InvokeVoidAsync("App.setDefaultAtomShape");
        }

        await JSRuntime.InvokeVoidAsync("App.startMusic");
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            StateContainer.OnChange -= StateHasChangedAsync;
        }
    }

    async Task StateHasChangedAsync()
    {
        await InvokeAsync(StateHasChanged);
    }

    async Task InitializeLocalStorageId()
    {
        var localStorageIdResult = 
            await ProtectedLocalStore.GetAsync<Guid>(
                Constants.StorageKeys.LocalStorageId);

        if (!localStorageIdResult.Success)
        {
            var localStorageId = Guid.CreateVersion7();

            await ProtectedLocalStore.SetAsync(
                Constants.StorageKeys.LocalStorageId,
                localStorageId);
        }
    }
}