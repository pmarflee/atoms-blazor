using Atoms.Core.Test;
using Atoms.UseCases.CreateNewGame;
using Atoms.UseCases.PlayerMove;
using Atoms.Core.DTOs.Notifications;
using Atoms.UseCases.UpdateGameFromPlayerMoveNotification;
using Atoms.Core.DTOs.Notifications.SignalR;

namespace Atoms.Web.Components.Shared;

public class BoardComponent : Component2Base, IDisposable, IAsyncDisposable
{
    const int AtomExplodedDelay = 50;
    const int PlayerMovedDelay = 300;

    [Inject]
    GameStateContainer StateContainer { get; set; } = default!;

    [Inject]
    protected NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    INotificationService NotificationService { get; set; } = default!;

    [Inject]
    ILogger<BoardComponent> Logger { get; set; } = default!;

    [Parameter]
    public EventCallback<CellClickEventArgs> OnCellClicked { get; set; }

    [Parameter]
    public int? Debug { get; set; }

    bool _disableClicks;
    readonly CancellationToken _cancellationToken = new();
    bool _handlingPlayerMove;

    protected async override Task OnInitializedAsync()
    {
        StateContainer.OnChange += StateChanged;
        StateContainer.OnGameSet += GameSet;
        StateContainer.OnGameStateChanged += GameStateChanged;

        NotificationService.OnPlayerMoved += PlayerMoved;
        NotificationService.OnGameReloadRequired += ReloadGameIfRequired;
        NotificationService.OnPlayerJoined += PlayerJoined;

        await NotificationService.Start(_cancellationToken);
    }

    protected async Task CellClicked(CellClickEventArgs eventArgs)
    {
        try
        {
            if (!CanPlayMove()) return;

            _disableClicks = true;

            await PlayMove(eventArgs.Position);
            await OnCellClicked.InvokeAsync(eventArgs);
        }
        finally
        {
            _disableClicks = false;
        }
    }

    protected async Task RematchClick()
    {
        if (Game is not null)
        {
            var hasSound = await BrowserStorageService.GetSound();
            var options = Game.CreateOptionsForRematch(hasSound);
            var userIdentity = new UserIdentity(UserId, await GetUserName());
            var request = new CreateNewGameRequest(
                Guid.NewGuid(), options, userIdentity);
            var response = await Mediator.Send(request);

            NavigationManager.NavigateToGame(response.Game);
        }
    }

    async Task AtomPlaced()
    {
        if (Game is null) return;

        if (Logger.IsEnabled(LogLevel.Debug))
        {
            Logger.LogDebug("Atom placed notification received. GameId='{GameId}'.",
                            Game.Id);
        }

        await UpdateGame();
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

        if (notification.CanHandle(Game, UserId, LocalStorageId))
        {
            var player = Game.GetPlayer(notification.PlayerId);

            await Notify($"{player} moved");
        }

        await SetCursor();

        if (Game.HasWinner)
        {
            await JSRuntime.InvokeVoidAsync("App.stopMusic");
        }

        if (!Game.ActivePlayer.IsHuman)
        {
            await DelayBetweenMoves();
        }
    }

    async Task PlayerJoined(PlayerJoined notification)
    {
        if (Game is null) return;

        if (!Game.PlayerBelongsToUser(notification.UserId,
                                      notification.LocalStorageId,
                                      UserId,
                                      LocalStorageId))
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
        if (Logger.IsEnabled(LogLevel.Debug))
        {
            Logger.LogDebug(
                "Received PlayerMoved notification. GameId='{GameId}', Row='{Row}', Column='{Column}., LastUpdatedDateUtc='{LastUpdatedDateUtc}', Id='{Id}'.",
                notification.GameId,
                notification.Row,
                notification.Column,
                notification.GameLastUpdatedDateUtc,
                notification.Id);
        }

        if (Game is not null && !_handlingPlayerMove)
        {
            try
            {
                _handlingPlayerMove = true;

                if (notification.GameLastUpdatedDateUtc == Game.LastUpdatedDateUtc)
                {
                    if (Logger.IsEnabled(LogLevel.Debug))
                    {
                        Logger.LogDebug("Update game from player move notification request. GameId='{GameId}'.",
                                        notification.GameId);
                    }

                    await Mediator.Send(
                        new UpdateGameFromPlayerMoveNotificationRequest(
                            Game, notification));
                }

                if (notification.GameLastUpdatedDateUtc >= Game.LastUpdatedDateUtc)
                {
                    if (Logger.IsEnabled(LogLevel.Debug))
                    {
                        Logger.LogDebug("Reload game after handling player move notification request. GameId='{GameId}'.",
                                        notification.GameId);
                    }

                    await ReloadGame();
                }
            }
            finally
            {
                _handlingPlayerMove = false;
            }
        }
    }

    Task GameStateChanged(GameStateChanged notification)
    {
        return notification switch
        {
            AtomPlaced _ => AtomPlaced(),
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

        try
        {
            _disableClicks = true;

            var moves = new Moves()
                .TakeWhile((move, index) =>
                    index < Debug!.Value && Game?.HasWinner == false);

            var gameService = new GameService();

            foreach (var (row, column) in moves)
            {
                if (Game is null) break;

                await gameService.PlayMove(
                    Game, Game.Board[row, column],
                    debug: true, notify: GameStateChanged);

                await DelayBetweenMoves();
            }
        }
        finally
        {
            _disableClicks = false;
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

        if (Game!.PlayerBelongsToUser(player, UserId, LocalStorageId))
        {
            playerNameClassNames.Add("name-highlight");
        }

        return string.Join(" ", playerNameClassNames);
    }

    protected Game? Game => StateContainer.Game;

    protected StorageId LocalStorageId => StateContainer.LocalStorageId;

    async Task PlayMove(Position? position = null)
    {
        PlayerMoveResponse response;

        do
        {
            response = await Mediator.Send(
                new PlayerMoveRequest(
                    Game!, position, Debug.HasValue,
                    UserId,
                    LocalStorageId));

            if (response.Result == PlayerMoveResult.GameStateHasChanged)
            {
                await ReloadGame();
            }
        } while (response.AllowRetry);
    }

    async Task SetCursor()
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

        await NotificationService.JoinGame(Game!, _cancellationToken);

        if (!isReload && !Game!.HasWinner && !Game!.ActivePlayer.IsHuman)
        {
            await PlayMove();
        }

        await SetCursor();
    }

    bool CanPlayMove()
    {
        if (_disableClicks) return false;

        return Game!.CanPlayMove(UserId, LocalStorageId);
    }

    async Task Notify(string message)
    {
        await JSRuntime.InvokeVoidAsync("App.notify", message);
    }

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        try
        {
            if (Game is not null)
            {
                await NotificationService.LeaveGame(Game, _cancellationToken);
            }
        }
        catch
        {
        }

        await NotificationService.DisposeAsync();
    }
}
