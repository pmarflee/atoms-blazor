namespace Atoms.UseCases.PlayerMove.Rebus;

public record PlayerMoveMessage(Guid GameId, int? Row, int? Column, DateTime LastUpdatedDateUtc);
