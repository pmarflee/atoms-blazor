namespace Atoms.Core.DTOs;

public sealed class GameInviteDTO
{
    public Guid GameId { get; init; }
    public Guid PlayerId { get; init; }
}
