namespace Atoms.UseCases.CreateDebugGame;

public class CreateDebugGameRequest(
    Guid gameId, int moves, StorageId localStorageId) 
    : IRequest<CreateDebugGameResponse>
{
    public Guid GameId { get; } = gameId;
    public int Moves { get; } = moves;
    public StorageId LocalStorageId { get; } = localStorageId;
}
