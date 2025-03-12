using Atoms.Core.Test;
using Atoms.UseCases.PlayerMove;
using Atoms.UseCases.Shared.Notifications;
using Atoms.Web.CustomEvents;
using MediatR.Courier;

namespace Atoms.Web.Components.Shared;

public class BoardComponent : Component2Base, IDisposable
{
    const int Delay = 50;

    [Inject]
    ICourier Courier { get; set; } = default!;

    [Inject]
    IJSRuntime JSRuntime { get; set; } = default!;

    [Inject]
    GameStateContainer StateContainer { get; set; } = default!;

    [Inject]
    IBrowserStorageService BrowserStorageService { get; set; } = default!;

    [CascadingParameter]
    ClaimsPrincipal? AuthenticatedUser { get; set; }

    [Parameter]
    public EventCallback OnPlayAgainClicked { get; set; }

    [Parameter]
    public int? Debug { get; set; }

    bool _disableClicks;

    protected override void OnInitialized()
    {
        StateContainer.OnChange += OnStateHasChanged;
        StateContainer.OnGameSet += OnGameSet;

        Courier.Subscribe<AtomPlaced>(AtomPlaced);
        Courier.Subscribe<AtomExploded>(AtomExploded);
    }

    protected async Task CellClicked(CellClickEventArgs eventArgs)
    {
        if (!await CanPlayMove()) return;

        await PlayMove(eventArgs.Cell, true);
    }

    protected async Task PlayAgainClick()
    {
        await OnPlayAgainClicked.InvokeAsync();
    }


    protected async Task AtomPlaced(AtomPlaced notification)
    {
        if (notification.Game.Id != StateContainer.Game!.Id) return;

        await UpdateGame();
    }

    protected async Task AtomExploded(AtomExploded notification)
    {
        if (notification.Game.Id != StateContainer.Game!.Id) return;

        await UpdateGame();

        await Task.Delay(Delay);
    }

    async Task UpdateGame()
    {
        await StateContainer.GameUpdated();
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
                await PlayMove(Game!.Board[row, column], false);
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

    protected Game? Game => StateContainer.Game;

    async Task PlayMove(Game.GameBoard.Cell? cell = null, bool updateCursor = true)
    {
        var response = await Mediator.Send(
            new PlayerMoveRequest(Game!, cell, Debug.HasValue));

        if (response.IsSuccessful)
        {
            if (updateCursor) await SetCursor();

            if (Game!.HasWinner)
            {
                await JSRuntime.InvokeVoidAsync("App.stopMusic");

                return;
            }

            if (!Game!.ActivePlayer.IsHuman)
            {
                await DelayBetweenMoves();
                await PlayMove();
            }
        }
    }

    async Task SetCursor()
    {
        if (await CanPlayMove())
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
        await Task.Delay(300);
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

            StateContainer.OnChange -= OnStateHasChanged;
            StateContainer.OnGameSet -= OnGameSet;
        }
    }

    async Task OnStateHasChanged()
    {
        await InvokeAsync(StateHasChanged);
    }

    async Task OnGameSet()
    {
        if (Debug.HasValue)
        {
            await PlayDebugGame();
        }
        else if (!Game!.ActivePlayer.IsHuman)
        {
            await PlayMove();
        }

        await SetCursor();
    }

    async Task<bool> CanPlayMove()
    {
        if (_disableClicks) return false;

        var authenticatedUserId = AuthenticatedUser.GetUserId();
        var localStorageId = await BrowserStorageService.GetStorageId();

        return Game!.CanPlayMove(authenticatedUserId, localStorageId);
    }
}
