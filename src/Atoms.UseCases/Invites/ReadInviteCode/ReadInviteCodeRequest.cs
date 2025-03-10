namespace Atoms.UseCases.Invites.ReadInviteCode;

public class ReadInviteCodeRequest(string code) : IRequest<ReadInviteCodeResponse>
{
    public string Code { get; } = code;
}
