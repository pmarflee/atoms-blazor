using FluentValidation;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Atoms.UseCases.Invites.ReadInviteCode;

public class ReadInviteCodeRequestHandler(
    IInviteSerializer inviteSerializer,
    ILogger<ReadInviteCodeRequestHandler> logger,
    IValidator<Invite> inviteValidator) 
    : IRequestHandler<ReadInviteCodeRequest, ReadInviteCodeResponse>
{
    public async Task<ReadInviteCodeResponse> Handle(
        ReadInviteCodeRequest request, CancellationToken cancellationToken)
    {
        ReadInviteCodeResponse response;

        try
        {
            var invite = inviteSerializer.Deserialize(request.Code);

            logger.LogInformation(
                "Read invite code '{code}' as json: '{json}'", 
                request.Code,
                JsonSerializer.Serialize(invite));

            var validationResult = await inviteValidator.ValidateAsync(invite, cancellationToken);

            if (!validationResult.IsValid)
            {
                return ReadInviteCodeResponse.Failure(
                    validationResult.Errors.First().ErrorMessage);
            }

            return ReadInviteCodeResponse.Success(invite);
        }
        catch (Exception ex)
        {
            response = ReadInviteCodeResponse.Failure("Invite code could not be read");

            logger.LogError(
                ex, 
                "Failed to read invite code: '{code}'",
                request.Code);
        }

        return response;
    }
}
