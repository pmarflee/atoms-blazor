namespace Atoms.UseCases.UpdateGameFromPlayerMoveNotification;

public record UpdateGameFromPlayerMoveNotificationRequest(
    Game Game,
    Core.DTOs.Notifications.SignalR.PlayerMoved Notification) : IRequest;
