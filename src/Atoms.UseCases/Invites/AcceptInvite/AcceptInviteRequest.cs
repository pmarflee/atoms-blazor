namespace Atoms.UseCases.Invites.AcceptInvite;

public class AcceptInviteRequest(Invite invite,
                                 UserId? userId,
                                 StorageId storageId,
                                 string name) 
    : IRequest<AcceptInviteResponse>
{
    public Invite Invite { get; } = invite;
    public UserId? UserId { get; } = userId;
    public StorageId StorageId { get; } = storageId;
    public string Name { get; } = name;
}
