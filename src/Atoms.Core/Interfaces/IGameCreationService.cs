namespace Atoms.Core.Interfaces;

public interface IGameCreationService
{
    Task<GameDTO> CreateGame(GameMenuOptions options,
                             UserIdentity userIdentity,
                             CancellationToken cancellationToken);
}
