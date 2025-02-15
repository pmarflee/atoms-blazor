namespace Atoms.UseCases.CreateNewGame;

public class CreateNewGameRequest(GameMenuOptions options, StorageId storageId) 
    : IRequest<CreateNewGameResponse>
{
    public GameMenuOptions Options { get; } = options;
    public StorageId StorageId { get; } = storageId;
}
