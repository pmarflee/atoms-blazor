namespace Atoms.Core.DTOs.Notifications;

public sealed record AtomExploded(
    Guid GameId,
    Guid PlayerId,
    Guid? RequestPlayerId,
    int Row,
    int Column,
    ExplosionState Explosion) 
    : CellUpdated(GameId, PlayerId, RequestPlayerId, Row, Column);
