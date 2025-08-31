namespace Atoms.UseCases.UpdateGameFromNotification;

public record UpdateGameFromAtomExplodedNotificationRequest(
    Game Game,
    AtomExploded Notification,
    UserId? UserId,
    StorageId? LocalStorageId) 
    : UpdateCellFromNotificationRequest<AtomExploded>(
        Game, Notification, UserId, LocalStorageId);
