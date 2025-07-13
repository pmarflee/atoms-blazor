using Atoms.Core.Delegates;

namespace Atoms.Infrastructure.Factories;

public static class GameFactory
{
    public static Game Create(
        CreateRng rngFactory, 
        CreatePlayerStrategy playerStrategyFactory,
        GameMenuOptions options,
        UserIdentity? userIdentity = null)
    {
        var rng = rngFactory.Invoke(options.GameId.GetHashCode(), 0);
        var optionsPlayers = options.Players
            .Take(options.NumberOfPlayers)
            .ToList();
        var firstHumanPlayer = optionsPlayers.FirstOrDefault(p => p.Type == PlayerType.Human);
        var players = optionsPlayers
            .Select(p => new Game.Player(
                p.Id, p.Number, p.Type,
                p == firstHumanPlayer ? userIdentity?.Id : null,
                p == firstHumanPlayer ? userIdentity?.Name : null,
                p == firstHumanPlayer ? userIdentity?.GetAbbreviatedName() : null,
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
