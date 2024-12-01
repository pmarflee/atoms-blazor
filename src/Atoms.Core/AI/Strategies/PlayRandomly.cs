using Atoms.Core.Interfaces;
using static Atoms.Core.Entities.Game.GameBoard;

namespace Atoms.Core.AI.Strategies;

public class PlayRandomly(IRandomNumberGenerator rng) : IPlayerStrategy
{
    public Cell? Choose(Game game)
    {
        Cell cell;

        do
        {
            var row = rng.Next(1, game.Rows + 1);
            var column = rng.Next(1, game.Columns + 1);

            cell = game.Board[row, column];
        } while (!game.CanPlaceAtom(cell));

        return cell;
    }
}
