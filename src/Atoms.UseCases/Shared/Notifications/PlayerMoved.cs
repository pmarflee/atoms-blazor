namespace Atoms.UseCases.Shared.Notifications;

public record PlayerMoved(
    Guid GameId,
    Guid PlayerId,
    Guid RequestPlayerId) 
    : GameStateChanged(GameId, PlayerId)
{
    public bool OriginatesFromRequestPlayer(
        Game game, UserId? userId, StorageId localStorageId)
    {
        var player = game.GetPlayer(PlayerId);

        if (!game.PlayerBelongsToUser(player, userId, localStorageId))
        {
            var requestPlayer = game.GetPlayer(RequestPlayerId);

            return requestPlayer.LocalStorageId == localStorageId;
        }

        return true;
    }
}
