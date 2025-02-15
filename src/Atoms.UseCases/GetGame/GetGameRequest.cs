namespace Atoms.UseCases.GetGame;

public class GetGameRequest(
    Guid gameId,
    StorageId storageId,
    string? userId = null) : IRequest<GetGameResponse>
{
    public Guid GameId { get; } = gameId;
    public StorageId StorageId { get; } = storageId;
    public string? UserId { get; } = userId;
}
