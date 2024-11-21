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

    protected string GetPlayerClassName(int? player) =>
        player.HasValue ? $"player{player - 1}" : "";

    protected string GetPlayerActiveClassName(Game.Player player) =>
        $"{(player == Game.ActivePlayer ? "active" : "")}";
}
