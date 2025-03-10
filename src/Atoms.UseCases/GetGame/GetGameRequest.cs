namespace Atoms.UseCases.GetGame;

public class GetGameRequest(
    Guid gameId,
    StorageId storageId,
    UserId? userId = null) : IRequest<GetGameResponse>
{
    public Guid GameId { get; } = gameId;
    public StorageId StorageId { get; } = storageId;
    public UserId? UserId { get; } = userId;
}
