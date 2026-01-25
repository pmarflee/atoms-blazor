using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;
using Atoms.Core.DTOs.Notifications.SignalR;
using Microsoft.Extensions.Logging;

namespace Atoms.Infrastructure.SignalR;

public class GameHub(ILogger<GameHub> logger) : Hub<IGameClient>
{
    public const string HubUrl = "/gamehub";

    static readonly ConcurrentDictionary<Guid, ConcurrentDictionary<string, byte>> _gameGroups = [];
    static readonly ConcurrentDictionary<string, ConcurrentDictionary<Guid, byte>> _connectionGroups = [];

    public async override Task OnDisconnectedAsync(Exception? exception)
    {
        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation(
                "Client disconnected. ConnectionId='{connectionId}'.",
                Context.ConnectionId);
        }

        await LeaveAllGames();

        await base.OnDisconnectedAsync(exception);
    }

    public async Task JoinGame(Guid gameId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, GroupName(gameId));

        var gameGroup = _gameGroups.GetOrAdd(gameId, _ => []);

        gameGroup.TryAdd(Context.ConnectionId, 0);

        var connectionGroup = _connectionGroups.GetOrAdd(
            Context.ConnectionId, _ => []);

        connectionGroup.TryAdd(gameId, 0);

        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation(
                "Game joined. GameId='{gameId}', ConnectionId='{connectionId}'.",
                gameId, Context.ConnectionId);
        }
    }

    public async Task LeaveGame(Guid gameId)
    {
        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation(
                "Leaving game. GameId='{gameId}', ConnectionId='{connectionId}'.",
                gameId, Context.ConnectionId);
        }

        var groupName = GroupName(gameId);

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

        if (_gameGroups.TryGetValue(gameId, out var gameGroup))
        {
            gameGroup.TryRemove(Context.ConnectionId, out _);

            if (gameGroup.IsEmpty)
            {
                _gameGroups.TryRemove(gameId, out _);
            }
        }
    }

#pragma warning disable CA1822 // Mark members as static
    public List<string> GetConnections(Guid gameId)
#pragma warning restore CA1822 // Mark members as static
    {
        return _gameGroups.TryGetValue(gameId, out var gameGroup) 
            ? [.. gameGroup.Keys]
            : [];
    }

    public async Task NotifyPlayerMoved(PlayerMoved notification)
    {
        await Clients
            .Group(GroupName(notification.GameId))
            .PlayerMoved(notification);
    }

    public async Task NotifyGameReloadRequired(GameReloadRequired notification)
    {
        await Clients.Group(GroupName(notification.GameId)).GameReloadRequired();
    }

    public async Task NotifyPlayerJoined(PlayerJoined notification)
    {
        await Clients
            .Group(GroupName(notification.GameId))
            .PlayerJoined(notification);
    }

    public async Task NotifyRematch(Rematch notification)
    {
        await Clients.Clients(notification.ConnectionIds)
            .Rematch(notification);
    }

    async Task LeaveAllGames()
    {
        if (_connectionGroups.TryGetValue(Context.ConnectionId, out var connectionGames))
        {
            foreach (var gameId in connectionGames.Keys)
            {
                await LeaveGame(gameId);

                connectionGames.Remove(gameId, out _);
            }

            _connectionGroups.Remove(Context.ConnectionId, out _);
        }
    }

    static string GroupName(Guid gameId) => gameId.ToString();
}
