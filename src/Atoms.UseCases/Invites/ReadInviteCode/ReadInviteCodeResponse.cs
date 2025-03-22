namespace Atoms.UseCases.Invites.ReadInviteCode;

public class ReadInviteCodeResponse
{
    public bool IsSuccessful { get; }
    public Invite? Invite { get; }
    public string? ErrorMessage { get; }

    ReadInviteCodeResponse(bool isSuccessful, Invite? invite = null, string? errorMessage = null)
    {
        IsSuccessful = isSuccessful;
        Invite = invite;
        ErrorMessage = errorMessage;
    }

    public static ReadInviteCodeResponse Success(Invite invite) => new(true, invite);
    public static ReadInviteCodeResponse Failure(string errorMessage) => new(false, errorMessage: errorMessage);
}
