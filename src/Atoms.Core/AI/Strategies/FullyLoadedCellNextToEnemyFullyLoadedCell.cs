using Atoms.Core.Interfaces;
using static Atoms.Core.Entities.Game.GameBoard;

namespace Atoms.Core.AI.Strategies;

public class FullyLoadedCellNextToEnemyFullyLoadedCell : IPlayerStrategy
{
    public Cell? Choose(Game game)
    {
        return (from cell in EnemyFullyLoadedCells(game)
                from neighbour in game.Board.GetNeighbours(cell)
                where neighbour.Player == game.ActivePlayer.Number
                && neighbour.IsFullyLoaded
                select cell).FirstOrDefault();
    }

    static IEnumerable<Cell> EnemyFullyLoadedCells(Game game)
    {
        return from cell in game.Board.Cells
               where cell.IsFullyLoaded
               && cell.Player != game.ActivePlayer.Number
               select cell;
    }
}
