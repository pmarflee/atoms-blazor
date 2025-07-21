namespace Atoms.UseCases.Menu.GameOptions;

public class CreateGameOptionsRequest(int numberOfPlayers,
                                      StorageId storageId,
                                      UserId? userId)
    : IRequest<GameOptionsResponse>
{
    public int NumberOfPlayers { get; } = numberOfPlayers;
    public StorageId StorageId { get; } = storageId;
    public UserId? UserId { get; } = userId;
}
