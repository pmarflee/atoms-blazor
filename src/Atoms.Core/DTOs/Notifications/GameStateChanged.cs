namespace Atoms.Core.DTOs.Notifications;

public abstract record GameStateChanged(Guid GameId, Guid PlayerId);
