namespace Atoms.UseCases.CreateNewGame;

public class CreateNewGameRequest(GameMenuOptions options,
                                  StorageId localStorageId,
                                  UserIdentity userIdentity)
    : IRequest<CreateNewGameResponse>
{
    public GameMenuOptions Options { get; } = options;
    public StorageId LocalStorageId { get; } = localStorageId;
    public UserIdentity UserIdentity { get; } = userIdentity;
}
