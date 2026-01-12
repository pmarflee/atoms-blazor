namespace Atoms.Core.ExtensionMethods;

public static class PlayerBelongsToUserExtensions
{
    extension(GameDTO gameDto)
    {
        public bool PlayerBelongsToUser(PlayerDTO playerDto,
                                        UserId? userId,
                                        StorageId? localStorageId)
        {
            return PlayerBelongsToUser(
                    playerDto.PlayerTypeId,
                    playerDto.UserId, playerDto.LocalStorageUserId,
                    userId, localStorageId, 
                    gameDto.UserId, gameDto.LocalStorageUserId);
        }
    }

    extension(Game game)
    {
        public bool PlayerBelongsToUser(Game.Player player,
                                        UserId? userId, StorageId? localStorageId)
        {
            return PlayerBelongsToUser(
                player.Type,
                player.UserId, player.LocalStorageId,
                userId, localStorageId,
                game.UserId, game.LocalStorageId);
        }

        public bool PlayerBelongsToUser(UserId? playerUserId, StorageId? playerLocalStorageId,
                                        UserId? userId, StorageId? localStorageId)
        {
            return PlayerBelongsToUser(
                playerUserId, playerLocalStorageId,
                userId, localStorageId,
                game.UserId, game.LocalStorageId);
        }
    }

    static bool PlayerBelongsToUser(PlayerType playerType,
                                    UserId? playerUserId, StorageId? playerLocalStorageId,
                                    UserId? userId, StorageId? localStorageId,
                                    UserId? gameUserId, StorageId gameLocalStorageId)
    {
        return playerType == PlayerType.Human
            && PlayerBelongsToUser(
                playerUserId, playerLocalStorageId,
                userId, localStorageId,
                gameUserId, gameLocalStorageId);
    }

    static bool PlayerBelongsToUser(UserId? playerUserId, StorageId? playerLocalStorageId,
                                    UserId? userId, StorageId? localStorageId,
                                    UserId? gameUserId, StorageId gameLocalStorageUserId)
    {
        if (userId is not null && playerUserId is not null)
        {
            return userId.Id == playerUserId.Id;
        }

        if (localStorageId is not null && playerLocalStorageId is not null)
        {
            return localStorageId == playerLocalStorageId;
        }

        return userId is not null && userId.Id == gameUserId
               || gameLocalStorageUserId == localStorageId;
    }
}
