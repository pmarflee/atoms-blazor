namespace Atoms.UseCases.Menu.GameOptions;

public class CreateGameOptionsRequest(Guid gameId,
                                      int numberOfPlayers,
                                      StorageId storageId,
                                      UserId? userId)
    : IRequest<GameOptionsResponse>
{
    public Guid GameId { get; } = gameId;
    public int NumberOfPlayers { get; } = numberOfPlayers;
    public StorageId StorageId { get; } = storageId;
    public UserId? UserId { get; } = userId;
}
