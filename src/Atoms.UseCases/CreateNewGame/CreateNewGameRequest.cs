namespace Atoms.UseCases.CreateNewGame;

public class CreateNewGameRequest(Guid gameId,
                                  GameMenuOptions options,
                                  StorageId localStorageId,
                                  UserIdentity userIdentity)
    : IRequest<CreateNewGameResponse>
{
    public Guid GameId { get; } = gameId;
    public GameMenuOptions Options { get; } = options;
    public StorageId LocalStorageId { get; } = localStorageId;
    public UserIdentity UserIdentity { get; } = userIdentity;
}
