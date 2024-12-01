using Atoms.Core.Interfaces;
using static Atoms.Core.Entities.Game.GameBoard;

namespace Atoms.Core.AI.Strategies;

public class ChooseCornerCell(IRandomNumberGenerator rng) : IPlayerStrategy
{
    public Cell? Choose(Game game)
    {
        if (rng.NextDouble() >= 0.8) return null;

        List<Cell> cornerCells =
            [
                game.Board[1, 1],
                game.Board[1, game.Columns],
                game.Board[game.Rows, 1],
                game.Board[game.Rows, game.Columns]
            ];

        var dangerCells = game.DangerCells;

        return cornerCells.FirstOrDefault(
            cell => cell.Atoms == 0 && 
                    !dangerCells.Contains(cell));
    }
}
