namespace Atoms.Core.DTOs.Notifications;

public record PlayerMoved(
    Guid GameId,
    Guid PlayerId,
    Guid? RequestPlayerId) 
    : GameStateChanged(GameId, PlayerId)
{
    public bool CanHandle(Game game, UserId? userId, StorageId localStorageId)
    {
        if (RequestPlayerId is null) return false;

        var requestPlayer = game.GetPlayer(RequestPlayerId.Value);

        if (!requestPlayer.IsHuman) return false;

        var player = game.GetPlayer(PlayerId);

        return !game.PlayerBelongsToUser(player, userId, localStorageId)
               && requestPlayer.LocalStorageId != localStorageId;
    }
}
