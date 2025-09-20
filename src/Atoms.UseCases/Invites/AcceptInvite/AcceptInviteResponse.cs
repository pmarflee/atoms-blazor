namespace Atoms.UseCases.Invites.AcceptInvite;

public record AcceptInviteResponse(Guid? GameId = null, string? ErrorMessage = null)
{
    public bool Success => GameId.HasValue;
}
