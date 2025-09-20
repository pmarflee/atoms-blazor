using Atoms.Core.Test;
using Atoms.UseCases.PlayerMove;
using Atoms.UseCases.Shared.Notifications;
using Atoms.UseCases.UpdateGameFromNotification;
using MediatR.Courier;

namespace Atoms.Web.Components.Shared;

public class BoardComponent : Component2Base, IDisposable
{
    const int AtomExplodedDelay = 50;
    const int PlayerMovedDelay = 300;

    [Inject]
    ICourier Courier { get; set; } = default!;

    [Inject]
    GameStateContainer StateContainer { get; set; } = default!;

    [Inject]
    protected IInviteSerializer InviteSerializer { get; set; } = default!;

    [Inject]
    protected NavigationManager NavigationManager { get; set; } = default!;

    [Parameter]
    public EventCallback OnPlayAgainClicked { get; set; }

    [Parameter]
    public EventCallback<CellClickEventArgs> OnCellClicked { get; set; }

    [Parameter]
    public int? Debug { get; set; }

    bool _disableClicks;

    protected override Task OnInitializedAsync()
    {
        StateContainer.OnChange += OnStateHasChanged;
        StateContainer.OnGameSet += OnGameSet;

        Courier.Subscribe<AtomPlaced>(AtomPlaced);
        Courier.Subscribe<AtomExploded>(AtomExploded);
        Courier.Subscribe<PlayerMoved>(PlayerMoved);
        Courier.Subscribe<GameSaved>(GameSaved);
        Courier.Subscribe<PlayerJoined>(PlayerJoined);

        return Task.CompletedTask;
    }

    protected async Task CellClicked(CellClickEventArgs eventArgs)
    {
        if (!CanPlayMove()) return;

        await PlayMove(eventArgs.Cell);
        await OnCellClicked.InvokeAsync(eventArgs);
    }

    protected async Task PlayAgainClick()
    {
        await OnPlayAgainClicked.InvokeAsync();
    }

    protected async Task AtomPlaced(AtomPlaced notification)
    {
        var game = Game!;

        if (notification.CanHandle(game))
        {
            await Mediator.Send(
                new UpdateGameFromAtomPlacedNotificationRequest(
                    game, notification, UserId, LocalStorageId));

            await UpdateGame();
        }
    }

    protected async Task AtomExploded(AtomExploded notification)
    {
        var game = Game!;

        if (notification.CanHandle(game))
        {
            await Mediator.Send(
                new UpdateGameFromAtomExplodedNotificationRequest(
                    game, notification, UserId, LocalStorageId));

            await UpdateGame();

            await Task.Delay(AtomExplodedDelay);
        }
    }

    protected async Task PlayerMoved(PlayerMoved notification)
    {
        var game = Game!;

        if (notification.CanHandle(game))
        {
            await UpdateGame();

            if (notification.CanHandle(game, UserId, LocalStorageId))
            {
                var player = game.GetPlayer(notification.PlayerId);

                await Notify($"{player} moved");
            }

            await SetCursor();

            if (game.HasWinner)
            {
                await JSRuntime.InvokeVoidAsync("App.stopMusic");
            }

            if (!game.ActivePlayer.IsHuman)
            {
                await DelayBetweenMoves();
            }
        }
    }

    protected async Task GameSaved(GameSaved notification)
    {
        var game = Game!;

        if (notification.CanHandle(game))
        {
            var player = game.GetPlayer(notification.PlayerId);    

            if (!player.IsHuman
                || !game.PlayerBelongsToUser(player, UserId, LocalStorageId))
            {
                await ReloadGame();
            }
        }
    }

    protected async Task PlayerJoined(PlayerJoined notification)
    {
        await ReloadGame();

        var game = Game!;
        var player = game.GetPlayer(notification.PlayerId);

        await Notify($"{player} joined");
    }

    async Task UpdateGame()
    {
        await StateContainer.GameUpdated();
    }

    async Task ReloadGame()
    {
        await StateContainer.GameReloadRequired();
    }

    async Task PlayDebugGame()
    {
        try
        {
            _disableClicks = true;

            var moves = new Moves()
                .TakeWhile((move, index) =>
                    index < Debug!.Value && !Game!.HasWinner);

            foreach (var (row, column) in moves)
            {
                await PlayMove(Game!.Board[row, column]);
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

    protected async Task CopyInviteToClipboard(Uri url)
    {
        await JSRuntime.InvokeVoidAsync("App.copyToClipboard", url.ToString());
    }

    async Task PlayMove(Game.GameBoard.Cell? cell = null)
    {
        var response = await Mediator.Send(
            new PlayerMoveRequest(
                Game!, cell, Debug.HasValue,
                UserId,
                LocalStorageId));

        if (response.Result == PlayerMoveResult.GameStateHasChanged)
        {
            await ReloadGame();
        }
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
            Courier.UnSubscribe<AtomPlaced>(AtomPlaced);
            Courier.UnSubscribe<AtomExploded>(AtomExploded);
            Courier.UnSubscribe<PlayerMoved>(PlayerMoved);
            Courier.UnSubscribe<GameSaved>(GameSaved);
            Courier.UnSubscribe<PlayerJoined>(PlayerJoined);

            StateContainer.OnChange -= OnStateHasChanged;
            StateContainer.OnGameSet -= OnGameSet;
        }
    }

    async Task OnStateHasChanged()
    {
        await InvokeAsync(StateHasChanged);
    }

    async Task OnGameSet(bool isReload)
    {
        if (Debug.HasValue)
        {
            await PlayDebugGame();
        }
        else if (!isReload && !Game!.HasWinner && !Game!.ActivePlayer.IsHuman)
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
}
