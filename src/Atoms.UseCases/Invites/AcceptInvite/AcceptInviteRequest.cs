namespace Atoms.UseCases.Invites.AcceptInvite;

public class AcceptInviteRequest(Invite invite, UserIdentity userIdentity) 
    : IRequest<AcceptInviteResponse>
{
    public Invite Invite { get; } = invite;
    public UserIdentity UserIdentity { get; } = userIdentity;
}
