namespace Atoms.UseCases.Invites.AcceptInvite;

public record AcceptInviteResponse(bool Success,
                                   PlayerDTO? Player = null,
                                   string? ErrorMessage = null);
