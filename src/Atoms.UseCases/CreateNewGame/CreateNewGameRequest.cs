namespace Atoms.UseCases.CreateNewGame;

public record CreateNewGameRequest(
    GameMenuOptions Options,
    UserIdentity UserIdentity)
    : IRequest<CreateNewGameResponse>;
