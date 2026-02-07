namespace Atoms.Core.DTOs.Notifications.SignalR;

public record PlayerJoined(Guid GameId, Guid PlayerId, 
                           string? UserId, Guid VisitorId,
                           string PlayerDescription);
