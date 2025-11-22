namespace Atoms.Core.DTOs.Notifications.SignalR;

public record PlayerMoved(Guid GameId,
                          int? Row,
                          int? Column,
                          DateTime LastUpdatedDateUtc)
{
    public Guid Id { get; init; } = Guid.NewGuid();
}
