using Atoms.Core.Interfaces;
using static Atoms.Core.Entities.Game.GameBoard;

namespace Atoms.Core.AI.Strategies;

public class GainAdvantageOverNeighbouringCell(IRandomNumberGenerator rng) 
    : IPlayerStrategy
{
    public Cell? Choose(Game game)
    {
        if (rng.NextDouble() >= 0.9) return null;

        var dangerCells = game.DangerCells;

        var priorityCells = (from cell in game.Board.Cells
                            where cell.Atoms > 0
                            && cell.Player != game.ActivePlayer.Number
                            from neighbour in game.Board.GetNeighbours(cell)
                            where game.CanPlaceAtom(neighbour)
                            && neighbour.FreeSpace <= cell.FreeSpace
                            && !dangerCells.Contains(neighbour)
                            select neighbour).ToList();

        return priorityCells.Count != 0 
            ? priorityCells[rng.Next(priorityCells.Count)] 
            : null;
    }
}
