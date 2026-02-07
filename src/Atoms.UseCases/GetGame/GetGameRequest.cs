namespace Atoms.UseCases.GetGame;

public class GetGameRequest(
    Guid gameId,
    VisitorId storageId,
    UserId? userId = null) : IRequest<GetGameResponse>
{
    public Guid GameId { get; } = gameId;
    public VisitorId VisitorId { get; } = storageId;
    public UserId? UserId { get; } = userId;
}
