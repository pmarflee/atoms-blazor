using Atoms.Core.Interfaces;
using static Atoms.Core.Entities.Game.GameBoard;

namespace Atoms.Core.AI.Strategies;

public class PlaySemiRandomlyAvoidingDangerCells(IRandomNumberGenerator rng) 
    : IPlayerStrategy
{
    public Cell? Choose(Game game)
    {
        var dangerCells = game.DangerCells;
        var tries = 0;
        Cell cell;

        do
        {
            var row = rng.Next(1, game.Rows + 1);
            var column = rng.Next(1, game.Columns + 1);

            cell = game.Board[row, column];
        } while ((dangerCells.Contains(cell) && tries++ < 20) || !game.CanPlaceAtom(cell));

        return cell;
    }
}
