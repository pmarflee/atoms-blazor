namespace Atoms.UseCases.Shared.Notifications;

public sealed record AtomPlaced(
    Guid GameId,
    Guid PlayerId,
    Guid RequestPlayerId,
    int Row,
    int Column) 
    : CellUpdated(GameId, PlayerId, RequestPlayerId, Row, Column);
