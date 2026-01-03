namespace Atoms.UseCases.PlayerMove;

public record PlayerMoveRequest(Game Game,
                               Position? Position = null,
                               bool Debug = false,
                               UserId? UserId = null,
                               StorageId? LocalStorageId = null)
    : IRequest<PlayerMoveResponse>;
