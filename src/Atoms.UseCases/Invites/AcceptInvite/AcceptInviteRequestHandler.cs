using Atoms.Core.DTOs.Notifications.SignalR;
using Atoms.Core.Utilities;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Atoms.UseCases.Invites.AcceptInvite;

public class AcceptInviteRequestHandler(
    IValidator<AcceptInviteRequest> acceptInviteRequestValidator,
    IDbContextFactory<ApplicationDbContext> dbContextFactory,
    IDateTimeService dateTimeService,
    IServiceScopeFactory serviceScopeFactory)
    : IRequestHandler<AcceptInviteRequest, AcceptInviteResponse>
{
    public async Task<AcceptInviteResponse> Handle(
        AcceptInviteRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await acceptInviteRequestValidator.ValidateAsync(
            request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var errorMessage = validationResult.Errors.First().ErrorMessage;

            return new(ErrorMessage: errorMessage);
        }

        using var dbContext = await dbContextFactory.CreateDbContextAsync(
            cancellationToken);

        var invite = request.Invite;
        var game = await dbContext.GetGameByPlayerId(invite.PlayerId, cancellationToken);
        var player = game!.Players.First(p => p.Id == invite.PlayerId);

        player.UserId = request.UserIdentity.Id;
        player.AbbreviatedName = request.UserIdentity.GetAbbreviatedName(game!.Players);
        player.VisitorId = request.VisitorId.Value;

        game.LastUpdatedDateUtc = dateTimeService.UtcNow;

        var visitor = (await dbContext.FindAsync<VisitorDTO>(
            request.VisitorId.Value))!;

        visitor.Name = request.UserIdentity.Name!;

        await dbContext.SaveChangesAsync(cancellationToken);

        var playerDescription = PlayerDescription.Build(
            player.Number, player.PlayerTypeId, request.UserIdentity.Name);

        await using var serviceScope = serviceScopeFactory.CreateAsyncScope();
        await using var notificationService = serviceScope.ServiceProvider.GetRequiredService<INotificationService>();

        await notificationService.Start(cancellationToken);

        await notificationService.NotifyPlayerJoined(
            new PlayerJoined(game.Id, player.Id, 
                             player.UserId, request.VisitorId.Value,
                             playerDescription),
            cancellationToken);

        return new(game.Id);
    }
}
