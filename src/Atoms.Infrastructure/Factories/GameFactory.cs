using Atoms.Core.Delegates;

namespace Atoms.Infrastructure.Factories;

public static class GameFactory
{
    public static GameDTO Create(
        CreateRng rngFactory,
        IDateTimeService dateTimeService,
        GameMenuOptions options,
        VisitorId visitorId,
        UserIdentity? userIdentity = null,
        Guid? gameId = null)
    {
        gameId ??= Guid.NewGuid();

        var rng = rngFactory.Invoke(gameId.Value.GetHashCode(), 0);

        return GameDTO.NewGame(
            gameId.Value,
            userIdentity,
            visitorId,
            options,
            rng,
            dateTimeService.UtcNow);
    }
}
