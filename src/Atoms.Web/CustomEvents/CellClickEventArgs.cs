using static Atoms.Core.Entities.Game;

namespace Atoms.Web.CustomEvents;

public class CellClickEventArgs(Game.GameBoard.Cell cell) : EventArgs
{
    public GameBoard.Cell Cell { get; } = cell;
}
