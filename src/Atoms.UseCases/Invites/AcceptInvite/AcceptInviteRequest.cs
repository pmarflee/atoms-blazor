namespace Atoms.UseCases.Invites.AcceptInvite;

public class AcceptInviteRequest(Invite invite, UserId? userId, string name) 
    : IRequest<AcceptInviteResponse>
{
    public Invite Invite { get; } = invite;
    public UserId? UserId { get; } = userId;
    public string Name { get; } = name;
}
