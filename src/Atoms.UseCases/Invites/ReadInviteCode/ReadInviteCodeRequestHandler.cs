
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Atoms.UseCases.Invites.ReadInviteCode;

public class ReadInviteCodeRequestHandler(
    IInviteSerializer inviteSerializer,
    ILogger<ReadInviteCodeRequestHandler> logger) 
    : IRequestHandler<ReadInviteCodeRequest, ReadInviteCodeResponse>
{
    public Task<ReadInviteCodeResponse> Handle(ReadInviteCodeRequest request,
                                               CancellationToken cancellationToken)
    {
        ReadInviteCodeResponse response;

        try
        {
            var invite = inviteSerializer.Deserialize(request.Code);

            response = ReadInviteCodeResponse.Success(invite);

            logger.LogInformation(
                "Read invite code '{code}' as json: '{json}'", 
                request.Code,
                JsonSerializer.Serialize(invite));
        }
        catch (Exception ex)
        {
            response = ReadInviteCodeResponse.Failure();

            logger.LogError(
                ex, 
                "Failed to read invite code: '{code}'",
                request.Code);
        }

        return Task.FromResult(response);
    }
}
