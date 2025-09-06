using FluentValidation;

namespace Atoms.UseCases.Invites.AcceptInvite;

public class AcceptInviteRequestHandler(
    IMediator mediator,
    ILocalStorageUserService localStorageUserService,
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
        var localStorageId = await localStorageUserService.GetOrAddLocalStorageId(cancellationToken);

        player.UserId = request.UserIdentity.Id;
        player.AbbreviatedName = request.UserIdentity.GetAbbreviatedName(game!.Players);
        player.LocalStorageUserId = localStorageId.Value;

        game.LastUpdatedDateUtc = dateTimeService.UtcNow;

        var localStorageUser = (await dbContext.FindAsync<LocalStorageUserDTO>(localStorageId.Value))!;

        localStorageUser.Name = request.UserIdentity.Name!;

        await dbContext.SaveChangesAsync(cancellationToken);

        await mediator.Publish(
            new PlayerJoined(game.Id, player.Id),
            cancellationToken);

        return new(true);
    }
}
