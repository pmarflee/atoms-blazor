using static Atoms.Core.Entities.Game.GameBoard;

namespace Atoms.Core.Interfaces;

public interface IPlayerStrategy
{
    Cell? Choose(Game game);
}
