namespace Atoms.UseCases.CreateNewGame;

public record CreateNewGameRequest(
    Guid GameId,
    GameMenuOptions Options,
    UserIdentity UserIdentity)
    : IRequest<CreateNewGameResponse>;
