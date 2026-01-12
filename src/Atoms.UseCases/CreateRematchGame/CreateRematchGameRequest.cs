namespace Atoms.UseCases.CreateRematchGame;

public record CreateRematchGameRequest(Guid GameId,
                                       UserIdentity UserIdentity,
                                       List<string>? OpponentConnectionIds = null) 
    : IRequest<CreateRematchGameResponse>;
