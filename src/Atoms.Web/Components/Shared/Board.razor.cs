using Atoms.Core.Test;
using Atoms.UseCases.PlayerMove;
using Atoms.Core.DTOs.Notifications;
using Atoms.UseCases.UpdateGameFromPlayerMoveNotification;
using Atoms.Core.DTOs.Notifications.SignalR;
using Atoms.UseCases.CreateRematchGame;

namespace Atoms.Web.Components.Shared;

public class BoardComponent : Component2Base, IDisposable, IAsyncDisposable
{
    const int AtomExplodedDelay = 50;
    const int PlayerMovedDelay = 300;
    const int AtomPlacedDelay = 300;

    [CascadingParameter]
    Guid GameId { get; set; }

    [Inject]
    GameStateContainer StateContainer { get; set; } = default!;

    [Inject]
    protected NavigationManager NavigationManager { get; set; } = default!;

    [CascadingParameter]
    INotificationService NotificationService { get; set; } = default!;

    [Inject]
    ILogger<BoardComponent> Logger { get; set; } = default!;

    [Parameter]
    public EventCallback<CellClickEventArgs> OnCellClicked { get; set; }

    [Parameter]
    public int? Debug { get; set; }

    bool _handlingPlayerMove;
    readonly CancellationToken _cancellationToken = new();
    List<string>? _opponentConnectionIds;

    protected async override Task OnInitializedAsync()
    {
        StateContainer.OnChange += StateChanged;
        StateContainer.OnGameSet += GameSet;
        StateContainer.OnGameStateChanged += GameStateChanged;

        NotificationService.OnPlayerMoved += PlayerMoved;
        NotificationService.OnGameReloadRequired += ReloadGameIfRequired;
        NotificationService.OnPlayerJoined += PlayerJoined;
    }

    protected async Task CellClicked(CellClickEventArgs eventArgs)
    {
        try
        {
            if (!CanPlayMove()) return;

            _handlingPlayerMove = true;

            await PlayMove(eventArgs.Position);
            await OnCellClicked.InvokeAsync(eventArgs);
        }
        catch
        {
            _handlingPlayerMove = false;

            throw;
        }
    }

    protected async Task RematchClick()
    {
        if (Game is not null)
        {
            var request = new CreateRematchGameRequest(
                Game.Id, VisitorId, new UserIdentity(UserId, UserName),
                _opponentConnectionIds ?? await GetGameConnections());
            var response = await Mediator.Send(request);

            NavigationManager.NavigateToGame(response.Game);
        }
    }

    protected bool CanHighlightCells => 
        Game is not null
        && !Game.PlayerBelongsToUser(Game.ActivePlayer, UserId, VisitorId);

    async Task AtomPlaced(AtomPlaced notification)
    {
        if (Game is null) return;

        if (Logger.IsEnabled(LogLevel.Debug))
        {
            Logger.LogDebug("Atom placed notification received. GameId='{GameId}'.",
                            Game.Id);
        }

        await UpdateGame();

        if (notification.Highlight
            && !Game.PlayerBelongsToUser(Game.ActivePlayer,
                                         UserId,
                                         VisitorId))
        {
            await Task.Delay(AtomPlacedDelay);
        }
    }

    async Task AtomExploded()
    {
        if (Game is null) return;

        if (Logger.IsEnabled(LogLevel.Debug))
        {
            Logger.LogDebug("Atom exploded notification received. GameId='{GameId}'.",
                            Game.Id);
        }

        await UpdateGame();

        await Task.Delay(AtomExplodedDelay);
    }

    async Task PlayerMoved(Core.DTOs.Notifications.PlayerMoved notification)
    {
        if (Game is null) return;

        await UpdateGame();

        if (notification.CanHandle(Game, UserId, VisitorId))
        {
            var player = Game.GetPlayer(notification.PlayerId);

            await Notify($"{player} moved");
        }

        if (Game.HasWinner)
        {
            _opponentConnectionIds = await GetGameConnections();

            await StopMusic();
        }

        if (!Game.ActivePlayer.IsHuman)
        {
            await DelayBetweenMoves();
        }
    }

    async Task StopMusic()
    {
        try
        {
            await JSRuntime.InvokeVoidAsync("App.stopMusic");
        }
        catch (TaskCanceledException)
        {
            // Ignore - client disconnected
        }
        catch (JSDisconnectedException)
        {
            // Ignore - client disconnected
        }
    }

    async Task PlayerJoined(PlayerJoined notification)
    {
        if (Game is null) return;

        if (!Game.PlayerBelongsToUser(notification.UserId,
                                      notification.VisitorId,
                                      UserId,
                                      VisitorId))
        {
            await Notify($"{notification.PlayerDescription} joined");
        }

        if (!_handlingPlayerMove)
        {
            await ReloadGame();
        }
    }

    async Task PlayerMoved(Core.DTOs.Notifications.SignalR.PlayerMoved notification)
    {
        if (Logger.IsEnabled(LogLevel.Information))
        {
            Logger.LogInformation(
                @"Received PlayerMoved notification. 
                GameId='{GameId}', Row='{Row}', Column='{Column}', 
                LastUpdatedDateUtc='{LastUpdatedDateUtc}', Id='{Id}'.",
                notification.GameId,
                notification.Row,
                notification.Column,
                notification.GameLastUpdatedDateUtc,
                notification.Id);
        }

        if (Game is not null)
        {
            try
            {
                _handlingPlayerMove = true;

                if (notification.GameLastUpdatedDateUtc == Game.LastUpdatedDateUtc)
                {
                    if (Logger.IsEnabled(LogLevel.Information))
                    {
                        Logger.LogInformation(
                            @"Update game from player move notification request. 
                            GameId='{GameId}', Row='{Row}', Column='{Column}', 
                            LastUpdatedDateUtc='{LastUpdatedDateUtc}', Id='{Id}'.",
                            notification.GameId,
                            notification.Row,
                            notification.Column,
                            notification.GameLastUpdatedDateUtc,
                            notification.Id);
                    }

                    await Mediator.Send(
                        new UpdateGameFromPlayerMoveNotificationRequest(
                            Game, notification));
                }

                if (notification.GameLastUpdatedDateUtc >= Game.LastUpdatedDateUtc)
                {
                    if (Logger.IsEnabled(LogLevel.Information))
                    {
                        Logger.LogInformation(
                            @"Reload game after handling player move notification request. 
                            GameId='{GameId}', Row='{Row}', Column='{Column}', 
                            LastUpdatedDateUtc='{LastUpdatedDateUtc}', Id='{Id}'.",
                            notification.GameId,
                            notification.Row,
                            notification.Column,
                            notification.GameLastUpdatedDateUtc,
                            notification.Id);
                    }

                    await ReloadGame();
                }
            }
            finally
            {
                _handlingPlayerMove = false;

                await SetCursor();
            }
        }
    }

    Task GameStateChanged(GameStateChanged notification)
    {
        return notification switch
        {
            AtomPlaced atomPlaced => AtomPlaced(atomPlaced),
            AtomExploded _ => AtomExploded(),
            Core.DTOs.Notifications.PlayerMoved playerMoved => PlayerMoved(playerMoved),
            _ => Task.CompletedTask,
        };
    }

    async Task UpdateGame()
    {
        await StateContainer.GameUpdated();
    }

    async Task ReloadGame()
    {
        await StateContainer.GameReloadRequired();
    }

    async Task ReloadGameIfRequired(GameReloadRequired notification)
    {
        if (Game is not null
            && notification.GameLastUpdatedDateUtc > Game.LastUpdatedDateUtc)
        {
            await ReloadGame();
        }
    }

    async Task PlayDebugGame()
    {
        if (Game is null) return;

        var moves = new Moves()
            .TakeWhile((move, index) =>
                index < Debug!.Value && Game?.HasWinner == false);

        var gameService = new GameMoveService();

        foreach (var (row, column) in moves)
        {
            if (Game is null) break;

            await gameService.PlayMove(
                Game, Game.Board[row, column],
                debug: true, notify: GameStateChanged);

            await DelayBetweenMoves();
        }
    }

    protected string GetPlayerClassName(int? player) =>
        player.HasValue ? $"player{player - 1}" : "";

    protected string GetPlayerActiveClassName(Game.Player player) =>
        $"{(player == Game!.ActivePlayer ? "active" : "")}";

    protected string GetPlayerDeadClassName(Game.Player player) =>
        $"{(player.IsDead ? "dead" : "")}";

    protected string GetPlayerClassNames(Game.Player player)
    {
        List<string> classNames =
            [
                "player",
                GetPlayerClassName(player.Number),
                GetPlayerActiveClassName(player),
                GetPlayerDeadClassName(player)
            ];

        return string.Join(
            " ",
            classNames.Where(cn => !string.IsNullOrEmpty(cn)));
    }

    protected string GetPlayerNameClassNames(Game.Player player)
    {
        List<string> playerNameClassNames = ["name"];

        if (!player.IsHuman)
        {
            playerNameClassNames.Add("name-cpu");
        }

        if (Game!.PlayerBelongsToUser(player, UserId, VisitorId))
        {
            playerNameClassNames.Add("name-highlight");
        }

        return string.Join(" ", playerNameClassNames);
    }

    protected Game? Game => StateContainer.Game;

    async Task PlayMove(Position? position = null)
    {
        PlayerMoveResponse response;

        do
        {
            response = await Mediator.Send(
                new PlayerMoveRequest(
                    Game!, position, Debug.HasValue,
                    UserId,
                    VisitorId));

            if (response.Result == PlayerMoveResult.GameStateHasChanged)
            {
                await ReloadGame();
            }
        } while (response.AllowRetry);
    }

    async Task SetCursor()
    {
        try
        {
            if (CanPlayMove())
            {
                await JSRuntime.InvokeVoidAsync(
                    "App.setCursor",
                    Game!.ActivePlayer.Number - 1);
            }
            else
            {
                await JSRuntime.InvokeVoidAsync("App.setDefaultCursor");
            }
        }
        catch (TaskCanceledException)
        {
            // Ignore - client disconnected
        }
        catch (JSDisconnectedException)
        {
            // Ignore - client disconnected
        }
    }

    static async Task DelayBetweenMoves()
    {
        await Task.Delay(PlayerMovedDelay);
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
            StateContainer.OnChange -= StateChanged;
            StateContainer.OnGameSet -= GameSet;
            StateContainer.OnGameStateChanged -= GameStateChanged;
        }
    }

    async Task StateChanged()
    {
        await InvokeAsync(StateHasChanged);
    }

    async Task GameSet(bool isReload)
    {
        if (Debug.HasValue)
        {
            await PlayDebugGame();

            return;
        }

        var game = Game!;

        await NotificationService.JoinGame(game.Id, _cancellationToken);

        if (!isReload && !Game!.HasWinner && !game.ActivePlayer.IsHuman)
        {
            await PlayMove();
        }

        await SetCursor();
    }

    bool CanPlayMove() => !Debug.HasValue
                          && !_handlingPlayerMove
                          && Game!.CanPlayMove(UserId, VisitorId);

    async Task Notify(string message)
    {
        try
        {
            await JSRuntime.InvokeVoidAsync("App.notify", message);
        }
        catch (TaskCanceledException)
        {
            // Ignore - client disconnected
        }
        catch (JSDisconnectedException)
        {
            // Ignore - client disconnected
        }
    }

    async Task<List<string>> GetGameConnections()
    {
        return await NotificationService.GetOpponentConnections(GameId);
    }

    async Task TryLeaveGame()
    {
        try
        {
            await NotificationService.LeaveGame(GameId, _cancellationToken);
        }
        catch (ObjectDisposedException) { /* Normal shutdown */ }
        catch (TaskCanceledException) { /* Normal shutdown */ }
        catch (Exception ex)
        {
            Logger.LogError(ex, "NotificationService.LeaveGame");
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (Logger.IsEnabled(LogLevel.Information))
        {
            Logger.LogInformation(
                "Disposing board instance. GameId='{gameId}'.",
                GameId);
        }

        await TryLeaveGame();

        NotificationService.OnPlayerMoved -= PlayerMoved;
        NotificationService.OnGameReloadRequired -= ReloadGameIfRequired;
        NotificationService.OnPlayerJoined -= PlayerJoined;

        GC.SuppressFinalize(this);
    }
}
