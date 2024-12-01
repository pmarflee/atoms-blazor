using static Atoms.Core.Entities.Game.GameBoard;

namespace Atoms.UseCases.PlayerMove;

public class PlayerMoveRequest(Game game, Cell? cell = null)
    : IRequest<PlayerMoveResponse>
{
    public Game Game { get; } = game;
    public Cell? Cell { get; } = cell;
}
