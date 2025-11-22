namespace Atoms.Core.DTOs.Notifications.SignalR;

public record AcknowledgePlayerMoved(Guid GameId,
                                     Guid NotificationId,
                                     string ConnectionId);
