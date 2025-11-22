using Atoms.Core.DTOs.Notifications.SignalR;
using Atoms.Core.Entities.Configuration;
using Atoms.Infrastructure.SignalR;
using Flurl;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;

namespace Atoms.Infrastructure.Services;

public class NotificationService : INotificationService, IAsyncDisposable
{
    readonly HubConnection _connection;

    public event Func<PlayerMoved, Task>? OnPlayerMoved;
    public event Func<AcknowledgePlayerMoved, Task>? OnAcknowledgePlayerMoved;
    public event Func<ClientDisconnected, Task>? OnClientDisconnected;
    public event Func<Task>? OnGameReloadRequired;
    public event Func<PlayerJoined, Task>? OnPlayerJoined;

    public NotificationService(
        IOptions<AppSettings> appSettings,
        NavigationManager navigationManager)
    {
        var baseHubUrl = appSettings.Value.BaseHubUrl
                         ?? navigationManager.ToAbsoluteUri("/").AbsoluteUri;

        _connection = new HubConnectionBuilder()
            .WithUrl(baseHubUrl.AppendPathSegment(GameHub.HubUrl))
            .WithAutomaticReconnect()
            .Build();

        _connection.On<PlayerMoved>(
            nameof(IGameClient.PlayerMoved),
            async notification =>
            {
                if (OnPlayerMoved is not null)
                {
                    await OnPlayerMoved.Invoke(notification);
                }
            });

        _connection.On<AcknowledgePlayerMoved>(
            nameof(IGameClient.AcknowledgePlayerMoved),
            async notification =>
            {
                if (OnAcknowledgePlayerMoved is not null)
                {
                    await OnAcknowledgePlayerMoved.Invoke(notification);
                }
            });

        _connection.On<ClientDisconnected>(
            nameof(IGameClient.ClientDisconnected),
            async notification =>
            {
                if (OnClientDisconnected is not null)
                {
                    await OnClientDisconnected.Invoke(notification);
                }
            });

        _connection.On<GameReloadRequired>(
            nameof(IGameClient.GameReloadRequired),
            async notification =>
            {
                if (OnGameReloadRequired is not null)
                {
                    await OnGameReloadRequired.Invoke();
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
    }

    public async Task Start(CancellationToken cancellationToken = default)
        => await _connection.StartAsync(cancellationToken);

    public async Task<List<string>> NotifyPlayerMoved(
        PlayerMoved notification, CancellationToken cancellationToken = default)
    {
        return await _connection.InvokeAsync<List<string>>(
            nameof(GameHub.NotifyPlayerMoved),
            notification,
            cancellationToken: cancellationToken);
    }

    public async Task AcknowledgePlayerMoved(AcknowledgePlayerMoved notification,
                                             CancellationToken cancellationToken = default)
    {
        await _connection.InvokeAsync(nameof(GameHub.AcknowledgePlayerMoved),
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
        Game game,
        CancellationToken cancellationToken = default)
    {
        await _connection.InvokeAsync(nameof(GameHub.JoinGame),
                                      game.Id,
                                      cancellationToken);
    }

    public async Task LeaveGame(
        Game game,
        CancellationToken cancellationToken = default)
    {
        await _connection.InvokeAsync(nameof(GameHub.LeaveGame),
                                      game.Id,
                                      cancellationToken);
    }

    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        return _connection.DisposeAsync();
    }
}
