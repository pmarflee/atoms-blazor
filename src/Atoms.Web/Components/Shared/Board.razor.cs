using Atoms.UseCases.PlaceAtom;
using Atoms.UseCases.Shared.Notifications;
using Atoms.Web.CustomEvents;
using MediatR.Courier;

namespace Atoms.Web.Components.Shared;

public class BoardComponent : Component2Base, IDisposable
{
    const int Delay = 50;

    [Inject]
    ICourier Courier { get; set; } = default!;

    [Parameter]
    public Game Game { get; set; } = default!;

    [Parameter]
    public EventCallback OnPlayAgainClicked { get; set; }

    protected override void OnInitialized()
    {
        Courier.Subscribe<GameStateChanged>(HandleNotification);
    }

    protected async Task CellClicked(CellClickEventArgs eventArgs)
    {
        if (Game.IsInProgress)
        {
            await Mediator.Send(new PlaceAtomRequest(Game, eventArgs.Cell));
        }
    }

    protected async Task PlayAgainClick()
    {
        await OnPlayAgainClicked.InvokeAsync();
    }

    protected async Task HandleNotification(GameStateChanged notification)
    {
        Game = notification.Game;
        StateHasChanged();

        await Task.Delay(Delay);
    }

    protected string GetPlayerClassName(int? player) =>
        player.HasValue ? $"player{player - 1}" : "";

    protected string GetPlayerActiveClassName(Game.Player player) =>
        $"{(player == Game.ActivePlayer ? "active" : "")}";

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

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            Courier.UnSubscribe<GameStateChanged>(HandleNotification);
        }
    }
}
