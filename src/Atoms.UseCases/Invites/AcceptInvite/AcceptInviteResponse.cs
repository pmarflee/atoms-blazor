namespace Atoms.UseCases.Invites.AcceptInvite;

public class AcceptInviteResponse
{
    AcceptInviteResponse(bool isSuccessful)
    {
        IsSuccessful = isSuccessful;
    }

    public bool IsSuccessful { get; }

    public static AcceptInviteResponse Success => new(true);
    public static AcceptInviteResponse Failure => new(false);

}
