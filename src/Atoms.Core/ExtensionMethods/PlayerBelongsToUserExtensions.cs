namespace Atoms.Core.ExtensionMethods;

public static class PlayerBelongsToUserExtensions
{
    extension(GameDTO gameDto)
    {
        public bool PlayerBelongsToUser(PlayerDTO playerDto,
                                        UserId? userId,
                                        VisitorId visitorId)
        {
            return PlayerBelongsToUser(
                    playerDto.PlayerTypeId,
                    playerDto.UserId, playerDto.VisitorId,
                    userId, visitorId, 
                    gameDto.UserId, gameDto.VisitorId);
        }
    }

    extension(Game game)
    {
        public bool PlayerBelongsToUser(Game.Player player,
                                        UserId? userId, VisitorId visitorId)
        {
            return PlayerBelongsToUser(
                player.Type,
                player.UserId, player.VisitorId,
                userId, visitorId,
                game.UserId, game.VisitorId);
        }

        public bool PlayerBelongsToUser(UserId? playerUserId, VisitorId playerVisitorId,
                                        UserId? userId, VisitorId visitorId)
        {
            return PlayerBelongsToUser(
                playerUserId, playerVisitorId,
                userId, visitorId,
                game.UserId, game.VisitorId);
        }
    }

    static bool PlayerBelongsToUser(PlayerType playerType,
                                    UserId? playerUserId, VisitorId? playerVisitorId,
                                    UserId? userId, VisitorId visitorId,
                                    UserId? gameUserId, VisitorId gameVisitorId)
    {
        return playerType == PlayerType.Human
            && PlayerBelongsToUser(
                playerUserId, playerVisitorId,
                userId, visitorId,
                gameUserId, gameVisitorId);
    }

    static bool PlayerBelongsToUser(UserId? playerUserId, VisitorId? playerVisitorId,
                                    UserId? userId, VisitorId visitorId,
                                    UserId? gameUserId, VisitorId gameVisitorId)
    {
        if (userId is not null && playerUserId is not null)
        {
            return userId.Id == playerUserId.Id;
        }

        if (playerVisitorId is not null)
        {
            return visitorId == playerVisitorId;
        }

        return userId is not null && userId.Id == gameUserId
               || gameVisitorId == visitorId;
    }
}
