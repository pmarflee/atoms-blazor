namespace Atoms.UseCases.UpdateGameFromNotification;

public record UpdateGameFromAtomPlacedNotificationRequest(
    Game Game,
    AtomPlaced Notification,
    UserId? UserId,
    StorageId? LocalStorageId) 
    : UpdateCellFromNotificationRequest<AtomPlaced>(
        Game, Notification, UserId, LocalStorageId);
