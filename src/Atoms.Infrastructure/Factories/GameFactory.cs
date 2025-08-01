﻿using Atoms.Core.Delegates;

namespace Atoms.Infrastructure.Factories;

public static class GameFactory
{
    public static Game Create(
        CreateRng rngFactory,
        CreatePlayerStrategy playerStrategyFactory,
        IInviteSerializer inviteSerializer,
        Guid gameId,
        GameMenuOptions options,
        StorageId localStorageId,
        UserIdentity? userIdentity = null)
    {
        var rng = rngFactory.Invoke(gameId.GetHashCode(), 0);
        var optionsPlayers = options.Players
            .Take(options.NumberOfPlayers)
            .ToList();
        var firstHumanPlayer = optionsPlayers.FirstOrDefault(p => p.Type == PlayerType.Human);

        string? CreateInviteLink(Guid playerId, GameMenuOptions.Player player)
        {
            if (player.Type == PlayerType.Human && player != firstHumanPlayer)
            {
                var invite = new Invite(gameId, playerId);

                return inviteSerializer.Serialize(invite);
            }

            return null;
        }

        List<Game.Player> players =
            [.. optionsPlayers
                .Select(p =>
                {
                    var playerId = Guid.NewGuid();

                    return new Game.Player(
                        playerId, p.Number, p.Type,
                        p == firstHumanPlayer ? userIdentity?.Id : null,
                        p == firstHumanPlayer ? userIdentity?.Name : null,
                        p == firstHumanPlayer ? userIdentity?.GetAbbreviatedName() : null,
                        playerStrategyFactory.Invoke(p.Type, rng),
                        p == firstHumanPlayer ? localStorageId : null,
                        CreateInviteLink(playerId, p));
                }) ];

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
