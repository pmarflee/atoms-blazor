using Atoms.UseCases.CreateDebugGame;
using Atoms.UseCases.GetGame;
using Microsoft.AspNetCore.SignalR.Client;

namespace Atoms.Web.Components.Pages;

public partial class GameComponent : Component2Base, IDisposable, IAsyncDisposable
{
    HubConnection? _hubConnection = default!;

    [Inject]
    NavigationManager Navigation { get; set; } = default!;

    [Inject]
    IBrowserStorageService BrowserStorageService { get; set; } = default!;

    [Inject]
    GameStateContainer StateContainer { get; set; } = default!;

    [Inject]
    IJSRuntime JSRuntime { get; set; } = default!;

    [CascadingParameter]
    ClaimsPrincipal? AuthenticatedUser { get; set; }

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
                new CreateDebugGameRequest(Debug.Value));

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

        _hubConnection.On<int>("PlayerMoved", 
            async playerNumber =>
            {
                await ReloadGame();
                await JSRuntime.InvokeVoidAsync("App.notifyPlayerMoved",
                                                playerNumber);
            });

        await _hubConnection.StartAsync();
        await _hubConnection.SendAsync("AddPlayer", GameId);

    }

    async Task LoadGame(bool isReload = false)
    {
        var userId = AuthenticatedUser.GetUserId();
        var storageId = await BrowserStorageService.GetOrAddStorageId();
        var response = await Mediator.Send(
            new GetGameRequest(GameId!.Value, storageId, userId));

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

        var hasSound = await BrowserStorageService.GetSound();

        if (hasSound)
        {
            await JSRuntime.InvokeVoidAsync("App.startMusic");
        }
    }

    protected void OnGoToHomePage()
    {
        Navigation.NavigateTo("/");
    }

    async Task StateHasChangedAsync()
    {
        await InvokeAsync(StateHasChanged);
    }

    async Task NotifyPlayerMoved(int playerNumber)
    {
        if (_hubConnection is not null && StateContainer.Game is not null)
        {
            await _hubConnection.SendAsync("SendNotification",
                                           StateContainer.Game.Id,
                                           playerNumber);
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