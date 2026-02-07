namespace Atoms.UseCases.CreateNewGame;

public record CreateNewGameRequest(
    GameMenuOptions Options,
    VisitorId VisitorId,
    UserIdentity UserIdentity)
    : IRequest<CreateNewGameResponse>;
