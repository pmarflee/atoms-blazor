using Atoms.Web.CustomEvents;

namespace Atoms.Web.Components.Shared;

public class BoardComponent : Component2Base
{
    [Parameter]
    public Game Game { get; set; } = default!;

    protected void CellClicked(CellClickEventArgs eventArgs)
    {
        if (Game.CanPlaceAtom(eventArgs.Cell))
        {
            Game.PlaceAtom(eventArgs.Cell);
        }
    }

    protected string GetPlayerClassName(Game.Player? player) =>
        player == null ? "" : $"player{player.Number - 1}";

    protected string GetPlayerActiveClassName(Game.Player player) =>
        $"{(player == Game.ActivePlayer ? "active" : "")}";
}
