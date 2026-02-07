namespace Atoms.UseCases.Menu.GameOptions;

public class CreateGameOptionsRequest(int numberOfPlayers,
                                      VisitorId storageId,
                                      UserId? userId)
    : IRequest<GameOptionsResponse>
{
    public int NumberOfPlayers { get; } = numberOfPlayers;
    public VisitorId VisitorId { get; } = storageId;
    public UserId? UserId { get; } = userId;
}
