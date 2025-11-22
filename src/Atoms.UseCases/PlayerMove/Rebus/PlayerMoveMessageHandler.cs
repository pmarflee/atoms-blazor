using Atoms.Core.Data.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Timeout;
using Rebus.Extensions;
using Rebus.Handlers;
using Rebus.Pipeline;

namespace Atoms.UseCases.PlayerMove.Rebus;

public class PlayerMoveMessageHandler(
    IDbContextFactory<ApplicationDbContext> applicationDbContextFactory,
    IDbContextFactory<ApplicationIdentityDbContext> applicationIdentityDbContextFactory,
    CreateRng rngFactory,
    CreatePlayerStrategy playerStrategyFactory,
    IGameService gameService,
    IDateTimeService dateTimeService,
    CreateNotificationService notificationServiceFactory,
    [FromKeyedServices("notify-player-moved")]ResiliencePipeline resiliencePipeline,
    ILogger<PlayerMoveMessageHandler> logger) 
    : IHandleMessages<PlayerMoveMessage>
{
    public async Task Handle(PlayerMoveMessage message)
    {
        var cancellationToken = MessageContext.Current.GetCancellationToken();

        using var applicationDbContext = 
            await applicationDbContextFactory.CreateDbContextAsync(
                cancellationToken);

        using var applicationIdentityDbContext =
            await applicationIdentityDbContextFactory.CreateDbContextAsync(
                cancellationToken);

        var gameDto = await applicationDbContext.GetGameById(
            message.GameId, cancellationToken);

        if (gameDto is null
            || gameDto.LastUpdatedDateUtc > message.LastUpdatedDateUtc)
        {
            return;
        }

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

        await using var notificationService = notificationServiceFactory.Invoke();

        var acknowledgementsPending = new HashSet<string>();

        Task RemoveFromPending(string connectionId)
        {
            acknowledgementsPending.Remove(connectionId);

            return Task.CompletedTask;
        }

        notificationService.OnAcknowledgePlayerMoved += 
            x => RemoveFromPending(x.ConnectionId);
        notificationService.OnClientDisconnected +=
            x => RemoveFromPending(x.ConnectionId);

        await notificationService.Start(cancellationToken);

        var connections = await notificationService.NotifyPlayerMoved(
            new(game.Id, message.Row, message.Column, lastUpdatedDateUtc),  
            cancellationToken);

        acknowledgementsPending.UnionWith(connections);

        try
        {
            resiliencePipeline.Execute(
                ctx => acknowledgementsPending.Count == 0,
                cancellationToken);
        }
        catch (TimeoutRejectedException)
        {
            logger.LogWarning(
                "Not all clients acknowledged the player move in time. " +
                "Pending acknowledgements from connections: '{Connections}'.",
                string.Join(", ", acknowledgementsPending));
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Error while waiting for client acknowledgements of player move. " +
                "Pending acknowledgements from connections: '{Connections}'.",
                string.Join(", ", acknowledgementsPending));
        }

        await notificationService.NotifyGameReloadRequired(
            new(game.Id), cancellationToken);
    }
}
