namespace Atoms.Core.DTOs.Notifications.SignalR;

public record GameReloadRequired(Guid GameId, DateTime LastUpdatedDateUtc);
