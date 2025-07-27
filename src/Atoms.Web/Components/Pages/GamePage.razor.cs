using Atoms.UseCases.CreateDebugGame;
using Atoms.UseCases.GetGame;
using Microsoft.AspNetCore.SignalR.Client;

namespace Atoms.Web.Components.Pages;

public partial class GameComponent : Component2Base, IDisposable, IAsyncDisposable
{
    HubConnection? _hubConnection = default!;
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
        StateContainer.OnPlayerMoved += NotifyPlayerMoved;

        if (GameId.HasValue)
        {
            await InitializeHub();
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

    private async Task InitializeHub()
    {
        _hubConnection = new HubConnectionBuilder()
                    .WithUrl(Navigation.ToAbsoluteUri("/gamehub"))
                    .Build();

        _hubConnection.On<string>("Notification",
            async message =>
            {
                await ReloadGame();
                await JSRuntime.InvokeVoidAsync("App.notify", message);
            });

        await _hubConnection.StartAsync();
        await _hubConnection.SendAsync("AddPlayer", GameId);

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
        await StateContainer.SetGame(game, isReload);
        await SetDisplayColourScheme(game.ColourScheme);
        await SetDisplayAtomShape(game.AtomShape);
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

    async Task NotifyPlayerMoved(int playerNumber, string? playerName)
    {
        if (_hubConnection is not null && StateContainer.Game is not null)
        {
            await _hubConnection.SendAsync(
                "Notify",
                StateContainer.Game.Id,
                $"Player {playerNumber}{(!string.IsNullOrEmpty(playerName) ? $" ({playerName})" : null)} moved");
        }
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
            StateContainer.OnPlayerMoved -= NotifyPlayerMoved;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_hubConnection is not null)
        {
            await _hubConnection.DisposeAsync();
        }

        GC.SuppressFinalize(this);
    }
}