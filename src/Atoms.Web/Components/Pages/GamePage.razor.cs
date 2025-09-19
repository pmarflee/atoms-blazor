using Atoms.UseCases.CreateDebugGame;
using Atoms.UseCases.GetGame;

namespace Atoms.Web.Components.Pages;

public partial class GameComponent : Component2Base, IDisposable
{
    bool _firstCellClicked;

    [Inject]
    NavigationManager Navigation { get; set; } = default!;

    [Inject]
    GameStateContainer StateContainer { get; set; } = default!;

    [Parameter]
    public Guid? GameId { get; set; }

    [SupplyParameterFromQuery]
    protected int? Debug { get; set; }

    protected async override Task OnInitializedAsync()
    {
        StateContainer.OnChange += StateHasChangedAsync;
        StateContainer.OnGameReloadRequired += ReloadGame;

        if (GameId.HasValue)
        {
            await LoadGame();
        }
        else if (Debug.HasValue)
        {
            var response = await Mediator.Send(
                new CreateDebugGameRequest(
                    Guid.NewGuid(), Debug.Value,
                    await GetOrAddStorageId()));

            await Task.Delay(10);
            await Initialize(response.Game);
        }
        else
        {
            Navigation.NavigateTo("/");
        }
    }

    async Task LoadGame(bool isReload = false)
    {
        var storageId = await GetOrAddStorageId();
        var response = await Mediator.Send(
            new GetGameRequest(GameId!.Value, storageId, UserId));

        if (response.Success)
        {
            Debug = null;

            await Initialize(response.Game!, isReload);
        }
        else
        {
            Navigation.NavigateTo("/");
        }
    }

    async Task ReloadGame() => await LoadGame(true);

    async Task Initialize(Game game, bool isReload = false)
    {
        var localStorageId = await GetOrAddStorageId();

        StateContainer.SetLocalStorageId(localStorageId);

        await SetDisplayColourScheme(game.ColourScheme);
        await SetDisplayAtomShape(game.AtomShape);
        await StateContainer.SetGame(game, isReload);
    }

    protected void GoToHomePage()
    {
        Navigation.NavigateTo("/");
    }

    protected async Task CellClicked()
    {
        if (!_firstCellClicked)
        {
            await TryPlayMusic();

            _firstCellClicked = true;
        }
    }

    async Task StateHasChangedAsync()
    {
        await InvokeAsync(StateHasChanged);
    }

    async Task TryPlayMusic()
    {
        var hasSound = await BrowserStorageService.GetSound();

        if (hasSound)
        {
            await JSRuntime.InvokeVoidAsync("App.startMusic");
        }
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
            StateContainer.OnGameReloadRequired -= ReloadGame;
        }
    }
}