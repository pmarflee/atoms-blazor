namespace Atoms.UseCases.UpdateGameFromNotification;

public abstract class UpdateCellFromNotificationRequestHandler<TRequest, TNotification>
    : IRequestHandler<TRequest>
    where TRequest : UpdateCellFromNotificationRequest<TNotification>, IRequest
    where TNotification : CellUpdated
{
    public Task Handle(TRequest request, CancellationToken cancellationToken)
    {
        var game = request.Game;
        var notification = request.Notification;
        var player = game.GetPlayer(notification.PlayerId);

        if (notification.CanHandle(
            game, request.UserId, request.LocalStorageId))
        {
            UpdateCell(game, notification, player);
        }

        return Task.CompletedTask;
    }

    protected abstract void UpdateCell(
        Game game, TNotification notification, Game.Player player);
}
