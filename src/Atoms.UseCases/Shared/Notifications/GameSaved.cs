namespace Atoms.UseCases.Shared.Notifications;

public sealed record GameSaved(Guid GameId, Guid PlayerId) 
    : GameStateChanged(GameId, PlayerId);
