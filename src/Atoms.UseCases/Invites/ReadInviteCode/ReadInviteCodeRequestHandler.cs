namespace Atoms.UseCases.Invites.ReadInviteCode;

public class ReadInviteCodeRequestHandler()
    : IRequestHandler<ReadInviteCodeRequest, ReadInviteCodeResponse>
{
    public Task<ReadInviteCodeResponse> Handle(
        ReadInviteCodeRequest request, CancellationToken cancellationToken)
    {
        var result = Guid.TryParse(request.Code, out var playerId)
            ? ReadInviteCodeResponse.Success(new(playerId))
            : ReadInviteCodeResponse.Failure("Invite code could not be read");

        return Task.FromResult(result);
    }
}
