using FluentValidation;

namespace Atoms.UseCases.Invites.AcceptInvite;

public class AcceptInviteRequestHandler(
    IBrowserStorageService browserStorageService,
    IValidator<Invite> inviteValidator,
    IDbContextFactory<ApplicationDbContext> dbContextFactory,
    IDateTimeService dateTimeService) 
    : IRequestHandler<AcceptInviteRequest, AcceptInviteResponse>
{
    public async Task<AcceptInviteResponse> Handle(AcceptInviteRequest request,
                                                   CancellationToken cancellationToken)
    {
        var invite = request.Invite;
        var validationResult = await inviteValidator.ValidateAsync(
            invite, cancellationToken);

        if (!validationResult.IsValid)
        {
            var errorMessage = validationResult.Errors.First().ErrorMessage;

            return new(false, ErrorMessage: errorMessage);
        }

        using var dbContext = await dbContextFactory.CreateDbContextAsync(
            cancellationToken);

        var game = await dbContext.GetGameById(invite.GameId, cancellationToken);
        var player = game!.Players.First(p => p.Id == invite.PlayerId);
        var localStorageId = await browserStorageService.GetOrAddStorageId();

        player.UserId = request.UserIdentity.Id;
        player.Name = request.UserIdentity.Name;
        player.AbbreviatedName = request.UserIdentity.GetAbbreviatedName(game!.Players);
        player.LocalStorageId = localStorageId.Value;

        game.LastUpdatedDateUtc = dateTimeService.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);

        await browserStorageService.SetUserName(request.UserIdentity.Name!);

        return new(true, player);
    }
}
