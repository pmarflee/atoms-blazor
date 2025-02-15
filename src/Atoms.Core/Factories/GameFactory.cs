using Atoms.Core.Interfaces;

namespace Atoms.Core.Factories;

public static class GameFactory
{
    public static Game Create(
        Func<int, int, IRandomNumberGenerator> rngFactory, 
        Func<PlayerType, IRandomNumberGenerator, IPlayerStrategy?> playerStrategyFactory,
        Guid gameId,
        GameMenuOptions options)
    {
        var rng = rngFactory.Invoke(gameId.GetHashCode(), 0);
        var players = options.Players
            .Take(options.NumberOfPlayers)
            .Select(p => new Game.Player(
                Guid.NewGuid(), p.Number, p.Type, p.UserId,
                playerStrategyFactory.Invoke(p.Type, rng)))
            .ToList();

        return new Game(gameId,
                        Constants.Rows,
                        Constants.Columns,
                        players,
                        players[0],
                        options.ColourScheme,
                        options.AtomShape,
                        rng);
    }
}
