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

    [Inject]
    ILogger<GamesPageComponent> Logger { get; set; } = default!;

    [Parameter]
    public Guid GameId { get; set; }

    [SupplyParameterFromQuery]
    protected int? Debug { get; set; }

    protected async override Task OnInitializedAsync()
    {
        StateContainer.OnChange += StateHasChangedAsync;
        StateContainer.OnGameReloadRequired += ReloadGame;

        if (Debug.HasValue)
        {
            var response = await Mediator.Send(
                new CreateDebugGameRequest(
                    GameId, Debug.Value,
                    VisitorId));

            await Task.Delay(10);
            await Initialize(response.Game);
        }
        else
        {
            await LoadGame();
        }
    }

    async Task LoadGame(bool isReload = false)
    {
        try
        {
            if (Logger.IsEnabled(LogLevel.Information))
            {
                Logger.LogInformation("Loading game. Game='{gameId}'.", GameId);
            }

            var response = await Mediator.Send(
                new GetGameRequest(GameId, VisitorId, UserId));

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
        catch (TaskCanceledException)
        {
            if (Logger.IsEnabled(LogLevel.Information))
            {
                Logger.LogWarning(
                    "Abort loading game, task was cancelled. Game='{gameId}'.",
                    GameId);
            }
        }
    }

    async Task ReloadGame() => await LoadGame(true);

    async Task Initialize(Game game, bool isReload = false)
    {
        await SetDisplayColourScheme(game.ColourScheme);
        await SetDisplayAtomShape(game.AtomShape);
        await StateContainer.SetGame(game, isReload);
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