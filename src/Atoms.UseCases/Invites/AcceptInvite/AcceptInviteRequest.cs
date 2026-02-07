namespace Atoms.UseCases.Invites.AcceptInvite;

public record AcceptInviteRequest(Invite Invite, VisitorId VisitorId, UserIdentity UserIdentity) 
    : IRequest<AcceptInviteResponse>;
