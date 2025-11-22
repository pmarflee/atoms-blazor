using Atoms.Core.DTOs.Notifications.SignalR;
using Atoms.Core.Utilities;
using FluentValidation;

namespace Atoms.UseCases.Invites.AcceptInvite;

public class AcceptInviteRequestHandler(
    ILocalStorageUserService localStorageUserService,
    IValidator<Invite> inviteValidator,
    IDbContextFactory<ApplicationDbContext> dbContextFactory,
    IDateTimeService dateTimeService,
    CreateNotificationService notificationServiceFactory)
    : IRequestHandler<AcceptInviteRequest, AcceptInviteResponse>
{
    public async Task<AcceptInviteResponse> Handle(
        AcceptInviteRequest request, CancellationToken cancellationToken)
    {
        var invite = request.Invite;
        var validationResult = await inviteValidator.ValidateAsync(
            invite, cancellationToken);

        if (!validationResult.IsValid)
        {
            var errorMessage = validationResult.Errors.First().ErrorMessage;

            return new(ErrorMessage: errorMessage);
        }

        using var dbContext = await dbContextFactory.CreateDbContextAsync(
            cancellationToken);

        var game = await dbContext.GetGameByPlayerId(invite.PlayerId, cancellationToken);
        var player = game!.Players.First(p => p.Id == invite.PlayerId);
        var localStorageId = await localStorageUserService.GetOrAddLocalStorageId(cancellationToken);

        player.UserId = request.UserIdentity.Id;
        player.AbbreviatedName = request.UserIdentity.GetAbbreviatedName(game!.Players);
        player.LocalStorageUserId = localStorageId.Value;

        game.LastUpdatedDateUtc = dateTimeService.UtcNow;

        var localStorageUser = (await dbContext.FindAsync<LocalStorageUserDTO>(localStorageId.Value))!;

        localStorageUser.Name = request.UserIdentity.Name!;

        await dbContext.SaveChangesAsync(cancellationToken);

        var playerDescription = PlayerDescription.Build(
            player.Number, player.PlayerTypeId, request.UserIdentity.Name);

        await using var notificationService = notificationServiceFactory.Invoke();

        await notificationService.Start(cancellationToken);

        await notificationService.NotifyPlayerJoined(
            new PlayerJoined(game.Id, player.Id, 
                             player.UserId, localStorageId.Value,
                             playerDescription),
            cancellationToken);

        return new(game.Id);
    }
}
