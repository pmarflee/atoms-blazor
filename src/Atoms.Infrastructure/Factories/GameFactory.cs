using Atoms.Core.Delegates;

namespace Atoms.Infrastructure.Factories;

public static class GameFactory
{
    public static Game Create(
        CreateRng rngFactory,
        CreatePlayerStrategy playerStrategyFactory,
        Guid gameId,
        GameMenuOptions options,
        StorageId localStorageId,
        UserIdentity? userIdentity = null)
    {
        var rng = rngFactory.Invoke(gameId.GetHashCode(), 0);
        var optionsPlayers = options.Players
            .Take(options.NumberOfPlayers)
            .ToList();
        var foundHumanPlayer = false;

        List<Game.Player> players = [.. optionsPlayers
            .Select(optionsPlayer =>
            {
                var playerId = Guid.NewGuid();
                UserIdentity? playerIdentity;
                StorageId? playerLocalStorageId;

                if (!options.IsRematch
                    && !foundHumanPlayer
                    && optionsPlayer.Type == PlayerType.Human)
                {
                    playerIdentity = userIdentity;
                    playerLocalStorageId = localStorageId;
                    foundHumanPlayer = true;
                }
                else
                {
                    playerIdentity = optionsPlayer.UserIdentity;
                    playerLocalStorageId = optionsPlayer.LocalStorageId;
                }

                return new Game.Player(
                    playerId,
                    optionsPlayer.Number,
                    optionsPlayer.Type,
                    playerIdentity?.Id,
                    playerIdentity?.Name,
                    playerIdentity?.GetAbbreviatedName(),
                    playerStrategyFactory.Invoke(optionsPlayer.Type, rng),
                    playerLocalStorageId);
            })];

        return new Game(gameId,
                        Constants.Rows,
                        Constants.Columns,
                        players,
                        players[0],
                        options.ColourScheme,
                        options.AtomShape,
                        rng,
                        localStorageId,
                        DateTime.UtcNow,
                        userId: userIdentity?.Id);
    }
}
