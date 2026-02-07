namespace Atoms.Core.Interfaces;

public interface IGameCreationService
{
    Task<GameDTO> CreateGame(GameMenuOptions options,
                             VisitorId visitorId,
                             UserIdentity userIdentity,
                             CancellationToken cancellationToken);
}
