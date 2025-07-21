namespace Atoms.UseCases.CreateDebugGame;

public class CreateDebugGameRequest(int moves, StorageId localStorageId) 
    : IRequest<CreateDebugGameResponse>
{
    public int Moves { get; } = moves;
    public StorageId LocalStorageId { get; } = localStorageId;
}
