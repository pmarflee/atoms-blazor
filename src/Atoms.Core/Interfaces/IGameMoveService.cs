using static Atoms.Core.Entities.Game.GameBoard;

namespace Atoms.Core.Interfaces;

public interface IGameMoveService
{
    Task PlayAllMoves(Game game, Cell? cell = null, Notify? notify = null);

    Task PlayMove(Game game, Cell cell, 
                  Game.Player? requestPlayer = null,
                  bool debug = false, Notify? notify = null);
}
