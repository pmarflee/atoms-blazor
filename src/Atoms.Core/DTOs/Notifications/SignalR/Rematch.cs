namespace Atoms.Core.DTOs.Notifications.SignalR;

public record Rematch(Guid GameId, List<string> ConnectionIds, string PlayerName);
