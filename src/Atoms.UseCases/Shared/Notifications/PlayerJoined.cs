namespace Atoms.UseCases.Shared.Notifications;

public sealed record PlayerJoined(Guid GameId, Guid PlayerId) 
    : GameStateChanged(GameId, PlayerId);
