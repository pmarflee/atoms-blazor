namespace Atoms.UseCases.UpdateGameFromNotification;

public abstract record UpdateCellFromNotificationRequest<TNotification>(
    Game Game,
    TNotification Notification,
    UserId? UserId,
    StorageId LocalStorageId) : IRequest
    where TNotification : CellUpdated;
