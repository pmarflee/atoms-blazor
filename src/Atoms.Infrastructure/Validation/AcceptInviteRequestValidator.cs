using Atoms.Core.Data;
using Atoms.UseCases.Invites.AcceptInvite;
using FluentValidation;

namespace Atoms.Infrastructure.Validation;

public class AcceptInviteRequestValidator : AbstractValidator<AcceptInviteRequest>
{
    public AcceptInviteRequestValidator(
        IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        RuleFor(x => x).CustomAsync(async (request, ctx, token) =>
        {
            var invite = request.Invite;
            var dbContext = await dbContextFactory.CreateDbContextAsync(token);
            var game = await dbContext.GetGameByPlayerId(invite.PlayerId, token);

            if (game is null)
            {
                ctx.AddFailure(nameof(Invite.PlayerId), "Game not found");

                return;
            }

            if (!game.IsActive)
            {
                ctx.AddFailure(nameof(Invite.PlayerId), "Game over");

                return;
            }

            var player = game.Players.First(p => p.Id == invite.PlayerId);

            if (player.UserId is not null || player.VisitorId is not null)
            {
                ctx.AddFailure(nameof(Invite.PlayerId), "Invite no longer valid");
            }

            var firstHumanPlayer = game.Players.First(p => p.PlayerTypeId == PlayerType.Human);

            if (request.VisitorId == firstHumanPlayer.VisitorId)
            {
                ctx.AddFailure(nameof(Invite.PlayerId), "Invite not accepted on same browser instance");
            }
        });
    }
}
