
namespace Atoms.UseCases.Shared.Notifications;

public abstract record CellUpdated(
    Guid GameId,
    Guid PlayerId,
    Guid RequestPlayerId,
    int Row,
    int Column) 
    : PlayerMoved(GameId, PlayerId, RequestPlayerId);
