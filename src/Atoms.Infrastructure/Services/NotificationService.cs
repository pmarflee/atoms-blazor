using Atoms.Core.DTOs.Notifications.SignalR;
using Atoms.Core.Entities.Configuration;
using Atoms.Infrastructure.SignalR;
using Flurl;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Channels;

namespace Atoms.Infrastructure.Services;

public class NotificationService : INotificationService, IAsyncDisposable
{
    readonly CancellationTokenSource _cts = new();
    readonly HubConnection _connection;
    readonly Channel<PlayerMoved> _playerMovedNotificationChannel;
    readonly ILogger<NotificationService> _logger;

    public event Func<PlayerMoved, Task>? OnPlayerMoved;
    public event Func<GameReloadRequired, Task>? OnGameReloadRequired;
    public event Func<PlayerJoined, Task>? OnPlayerJoined;
    public event Func<Rematch, Task>? OnRematch;

    public NotificationService(
        IOptions<AppSettings> appSettings,
        NavigationManager navigationManager,
        ILogger<NotificationService> logger)
    {
        _logger = logger;

        var baseHubUrl = appSettings.Value.BaseHubUrl
                         ?? navigationManager.ToAbsoluteUri("/").AbsoluteUri;

        _playerMovedNotificationChannel = Channel.CreateUnbounded<PlayerMoved>(
            new UnboundedChannelOptions { SingleReader = true });

        _connection = new HubConnectionBuilder()
            .WithUrl(baseHubUrl.AppendPathSegment(GameHub.HubUrl))
            .WithAutomaticReconnect()
            .Build();

        _connection.On<PlayerMoved>(
            nameof(IGameClient.PlayerMoved),
            async notification =>
            {
                if (logger.IsEnabled(LogLevel.Information))
                {
                    logger.LogInformation(
                        @"Received player moved notification. 
                        Game='{gameId}', Row='{row}', Column='{column}'.",
                        notification.GameId, notification.Row, notification.Column);
                }

                await _playerMovedNotificationChannel.Writer.WriteAsync(notification);
            });

        _connection.On<GameReloadRequired>(
            nameof(IGameClient.GameReloadRequired),
            async notification =>
            {
                if (OnGameReloadRequired is not null)
                {
                    await OnGameReloadRequired.Invoke(notification);
                }
            });

        _connection.On<PlayerJoined>(
            nameof(IGameClient.PlayerJoined),
            async notification =>
            {
                if (OnPlayerJoined is not null)
                {
                    await OnPlayerJoined.Invoke(notification);
                }
            });

        _connection.On<Rematch>(
            nameof(IGameClient.Rematch),
            async notification =>
            {
                if (OnRematch is not null)
                {
                    await OnRematch.Invoke(notification);
                }
            });
    }

    public async Task Start(CancellationToken cancellationToken = default)
    {
        await _connection.StartAsync(cancellationToken);

        _ = StartPlayerMovedNotificationConsumer(
            _playerMovedNotificationChannel.Reader,
            _cts.Token);
    }

    public async Task NotifyPlayerMoved(
        PlayerMoved notification, CancellationToken cancellationToken = default)
    {
        await _connection.InvokeAsync(
            nameof(GameHub.NotifyPlayerMoved),
            notification,
            cancellationToken: cancellationToken);
    }

    public async Task NotifyGameReloadRequired(
        GameReloadRequired notification,
        CancellationToken cancellationToken = default)
    {
        await _connection.InvokeAsync(nameof(GameHub.NotifyGameReloadRequired),
                                      notification,
                                      cancellationToken);
    }

    public async Task NotifyPlayerJoined(
        PlayerJoined notification,
        CancellationToken cancellationToken = default)
    {
        await _connection.InvokeAsync(nameof(GameHub.NotifyPlayerJoined),
                                      notification,
                                      cancellationToken);
    }

    public async Task JoinGame(
        Guid gameId,
        CancellationToken cancellationToken = default)
    {
        await _connection.InvokeAsync(nameof(GameHub.JoinGame),
                                      gameId,
                                      cancellationToken);
    }

    public async Task LeaveGame(
        Guid gameId,
        CancellationToken cancellationToken = default)
    {
        await _connection.InvokeAsync(nameof(GameHub.LeaveGame),
                                      gameId,
                                      cancellationToken);
    }

    public async Task<List<string>> GetOpponentConnections(
        Guid gameId,
        CancellationToken cancellationToken = default)
    {
        var connectionIds = await _connection.InvokeAsync<List<string>>(
            nameof(GameHub.GetConnections),
            gameId,
            cancellationToken);

        return [.. connectionIds.Where(id => id != _connection.ConnectionId)];
    }

    public async Task NotifyRematch(
        Rematch notification,
        CancellationToken cancellationToken = default)
    {
        await _connection.InvokeAsync(
            nameof(GameHub.NotifyRematch),
            notification,
            cancellationToken);
    }

    async Task StartPlayerMovedNotificationConsumer(
        ChannelReader<PlayerMoved> reader,
        CancellationToken cancellationToken)
    {
        try
        {
            await foreach (var notification in reader.ReadAllAsync(cancellationToken))
            {
                if (OnPlayerMoved is not null)
                {
                    try
                    {
                        if (_logger.IsEnabled(LogLevel.Information))
                        {
                            _logger.LogInformation(
                                @"Processing player moved notification.
                                Game='{gameId}', Row='{row}', Column='{column}',
                                LastUpdatedDate='{lastUpdatedDate}'.",
                                notification.GameId, notification.Row,
                                notification.Column, notification.GameLastUpdatedDateUtc);
                        }

                        await OnPlayerMoved.Invoke(notification);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error executing OnPlayerMoved event handler.");
                    }
                }
            }
        }
        catch (OperationCanceledException) { /* Normal shutdown */ }
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            await _cts.CancelAsync();

            _playerMovedNotificationChannel.Writer.TryComplete();

            _cts.Dispose();

            await _connection.DisposeAsync();
        }
        catch (ObjectDisposedException) { }

        GC.SuppressFinalize(this);
    }
}
