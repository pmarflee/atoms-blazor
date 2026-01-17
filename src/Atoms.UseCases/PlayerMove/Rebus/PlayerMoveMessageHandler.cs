using Atoms.Core.Data.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rebus.Extensions;
using Rebus.Handlers;
using Rebus.Pipeline;

namespace Atoms.UseCases.PlayerMove.Rebus;

public class PlayerMoveMessageHandler(
    IDbContextFactory<ApplicationDbContext> applicationDbContextFactory,
    IDbContextFactory<ApplicationIdentityDbContext> applicationIdentityDbContextFactory,
    CreateRng rngFactory,
    CreatePlayerStrategy playerStrategyFactory,
    IGameMoveService gameService,
    IDateTimeService dateTimeService,
    ILogger<PlayerMoveMessageHandler> logger,
    IServiceScopeFactory serviceScopeFactory) 
    : IHandleMessages<PlayerMoveMessage>
{
    public async Task Handle(PlayerMoveMessage message)
    {
        if (logger.IsEnabled(LogLevel.Debug))
        {
            logger.LogDebug(
                "Bus received message. GameId='{GameId}', Row='{Row}', Column='{Column}', " +
                "LastUpdatedDateUtc='{LastUpdatedDateUtc}'.",
                message.GameId, message.Row, message.Column,
                message.GameLastUpdatedDateUtc);
        }

        try
        {
            var cancellationToken = MessageContext.Current.GetCancellationToken();

            using var applicationDbContext =
                await applicationDbContextFactory.CreateDbContextAsync(
                    cancellationToken);

            var gameDto = await applicationDbContext.GetGameById(
                message.GameId, cancellationToken);

            if (gameDto is null
                || gameDto.LastUpdatedDateUtc > message.GameLastUpdatedDateUtc)
            {
                return;
            }

            using var applicationIdentityDbContext =
                await applicationIdentityDbContextFactory.CreateDbContextAsync(
                    cancellationToken);

            var lastUpdatedDateUtc = gameDto.LastUpdatedDateUtc;

            var game = await gameDto.ToEntity(
                rngFactory, playerStrategyFactory,
                applicationIdentityDbContext.GetUserById,
                applicationDbContext.GetLocalStorageUserById);

            var cell = message.Row.HasValue && message.Column.HasValue
                ? game.Board[message.Row.Value, message.Column.Value]
                : null;

            await gameService.PlayAllMoves(game, cell);

            gameDto.UpdateFromEntity(game, dateTimeService.UtcNow);

            await applicationDbContext.SaveChangesAsync(cancellationToken);

            await using var serviceScope = serviceScopeFactory.CreateAsyncScope();
            await using var notificationService = serviceScope.ServiceProvider.GetRequiredService<INotificationService>();

            await notificationService.Start(cancellationToken);
            await notificationService.JoinGame(game.Id, cancellationToken);

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Notifying players of move. GameId='{GameId}'.", game.Id);
            }

            await notificationService.NotifyPlayerMoved(
                new(game.Id, message.Row, message.Column, lastUpdatedDateUtc),
                cancellationToken);

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation(
                    "Notify reload required. GameId='{GameId}'.",
                    game.Id);
            }

            await notificationService.NotifyGameReloadRequired(
                new(game.Id, game.LastUpdatedDateUtc), cancellationToken);
        }
        catch (Exception ex)
        {
            if (logger.IsEnabled(LogLevel.Error))
            {
                logger.LogError(ex,
                    "Exception thrown when handling message. " +
                    "GameId='{GameId}', Row='{Row}', Column='{Column}', " +
                    "LastUpdatedDateUtc='{LastUpdatedDateUtc}'.",
                    message.GameId, message.Row, message.Column,
                    message.GameLastUpdatedDateUtc);
            }
        }
    }
}
