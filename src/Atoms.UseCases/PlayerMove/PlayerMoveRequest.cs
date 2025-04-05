using static Atoms.Core.Entities.Game.GameBoard;

namespace Atoms.UseCases.PlayerMove;

public class PlayerMoveRequest(Game game,
                               Cell? cell = null,
                               bool debug = false,
                               UserId? userId = null,
                               string? username = null,
                               StorageId? localStorageId = null)
    : IRequest<PlayerMoveResponse>
{
    public Game Game { get; } = game;
    public Cell? Cell { get; } = cell;
    public bool Debug { get; } = debug;
    public UserId? UserId { get; } = userId;
    public string? Username { get; } = username;
    public StorageId? LocalStorageId { get; } = localStorageId;
}
