using Atoms.Core.Delegates;

namespace Atoms.Infrastructure.Factories;

public static class GameFactory
{
    public static Game Create(
        CreateRng rngFactory, 
        CreatePlayerStrategy playerStrategyFactory,
        GameMenuOptions options)
    {
        var rng = rngFactory.Invoke(options.GameId.GetHashCode(), 0);
        var players = options.Players
            .Take(options.NumberOfPlayers)
            .Select(p => new Game.Player(
                p.Id, p.Number, p.Type, p.User, p.User?.Name,
                playerStrategyFactory.Invoke(p.Type, rng)))
            .ToList();

        return new Game(options.GameId,
                        Constants.Rows,
                        Constants.Columns,
                        players,
                        players[0],
                        options.ColourScheme,
                        options.AtomShape,
                        rng,
                        options.LocalStorageId!,
                        DateTime.UtcNow,
                        userId: options.UserId);
    }
}
