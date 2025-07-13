namespace Atoms.UseCases.CreateNewGame;

public class CreateNewGameRequest(GameMenuOptions options,
                                  UserIdentity userIdentity)
    : IRequest<CreateNewGameResponse>
{
    public GameMenuOptions Options { get; } = options;
    public UserIdentity UserIdentity { get; } = userIdentity;
}
