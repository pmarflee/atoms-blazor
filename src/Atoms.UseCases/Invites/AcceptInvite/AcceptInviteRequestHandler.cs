using Microsoft.Extensions.Logging;

namespace Atoms.UseCases.Invites.AcceptInvite;

public class AcceptInviteRequestHandler(
    IBrowserStorageService browserStorageService,
    IDbContextFactory<ApplicationDbContext> dbContextFactory,
    ILogger<AcceptInviteRequestHandler> logger) 
    : IRequestHandler<AcceptInviteRequest, AcceptInviteResponse>
{
    public async Task<AcceptInviteResponse> Handle(AcceptInviteRequest request,
                                                   CancellationToken cancellationToken)
    {
        using var dbContext = await dbContextFactory.CreateDbContextAsync(
            cancellationToken);

        var invite = request.Invite;
        var game = await dbContext.GetGameById(invite.GameId, cancellationToken);

        if (game is null)
        {
            logger.LogError("Unable to find game. Id: '{gameId}'.", invite.GameId);

            return AcceptInviteResponse.Failure;
        }

        var player = game.Players.FirstOrDefault(p => p.Id == invite.PlayerId);

        if (player is null)
        {
            logger.LogError(
                "Unable to find player. GameId: '{gameId}' PlayerId: '{playerId}'.",
                invite.GameId, invite.PlayerId);

            return AcceptInviteResponse.Failure;
        }

        var localStorageId = await browserStorageService.GetOrAddStorageId();

        player.UserId = request.UserId?.Id;
        player.LocalStorageId = localStorageId.Value;

        await dbContext.SaveChangesAsync(cancellationToken);

        await browserStorageService.SetUserName(request.Name);

        return AcceptInviteResponse.Success;
    }
}
