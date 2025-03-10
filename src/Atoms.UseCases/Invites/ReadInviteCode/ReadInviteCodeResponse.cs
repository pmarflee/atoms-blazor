namespace Atoms.UseCases.Invites.ReadInviteCode;

public class ReadInviteCodeResponse
{
    public bool IsSuccessful { get; }
    public Invite? Invite { get; }

    ReadInviteCodeResponse(bool isSuccessful, Invite? invite = null)
    {
        IsSuccessful = isSuccessful;
        Invite = invite;
    }

    public static ReadInviteCodeResponse Success(Invite invite) => new(true, invite);
    public static ReadInviteCodeResponse Failure() => new(false);
}
