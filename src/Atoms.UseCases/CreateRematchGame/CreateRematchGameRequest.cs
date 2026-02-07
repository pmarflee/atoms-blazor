namespace Atoms.UseCases.CreateRematchGame;

public record CreateRematchGameRequest(Guid GameId,
                                       VisitorId VisitorId,
                                       UserIdentity UserIdentity,
                                       List<string>? OpponentConnectionIds = null)
    : IRequest<CreateRematchGameResponse>;
