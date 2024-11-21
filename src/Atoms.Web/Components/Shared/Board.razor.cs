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

    protected override void OnInitialized()
    {
        Courier.Subscribe<GameStateChanged>(HandleNotification);
    }

    protected async Task CellClicked(CellClickEventArgs eventArgs)
    {
        await Mediator.Send(new PlaceAtomRequest(Game, eventArgs.Cell));
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
