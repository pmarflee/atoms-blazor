﻿using Atoms.Core.Data;
using FluentValidation;

namespace Atoms.Infrastructure.Validation;

public class InviteValidator : AbstractValidator<Invite>
{
    public InviteValidator(
        IDbContextFactory<ApplicationDbContext> dbContextFactory,
        IBrowserStorageService browserStorageService)
    {
        RuleFor(x => x).CustomAsync(async (invite, ctx, token) =>
        {
            var dbContext = await dbContextFactory.CreateDbContextAsync(token);
            var game = await dbContext.GetGameById(invite.GameId, token);

            if (game is null)
            {
                ctx.AddFailure(nameof(Invite.GameId), "Game not found");

                return;
            }

            if (!game.IsActive)
            {
                ctx.AddFailure(nameof(Invite.GameId), "Game over");

                return;
            }

            var player = game.Players.FirstOrDefault(p => p.Id == invite.PlayerId);

            if (player is null)
            {
                ctx.AddFailure(nameof(Invite.PlayerId), "Player not found");

                return;
            }

            if (player.UserId is not null || player.LocalStorageId is not null)
            {
                ctx.AddFailure(nameof(Invite.PlayerId), "Invite no longer valid");
            }

            var firstHumanPlayer = game.Players.First(p => p.Type == PlayerType.Human);
            var localStorageId = await browserStorageService.GetOrAddStorageId();

            if (localStorageId.Value == firstHumanPlayer.LocalStorageId)
            {
                ctx.AddFailure(nameof(Invite.PlayerId), "Invite not accepted on same browser instance");
            }
        });
    }
}
