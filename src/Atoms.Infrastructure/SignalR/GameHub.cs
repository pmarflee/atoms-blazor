using Atoms.Core.DTOs.Notifications.SignalR;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace Atoms.Infrastructure.SignalR;

public class GameHub : Hub<IGameClient>
{
    public const string HubUrl = "/gamehub";

    static readonly ConcurrentDictionary<Guid, ConcurrentDictionary<string, byte>> _groups = [];

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        foreach (var group in _groups.Values)
        {
            group.TryRemove(Context.ConnectionId, out _);
        }

        return base.OnDisconnectedAsync(exception);
    }

    public async Task JoinGame(Guid gameId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, GroupName(gameId));

        var group = _groups.GetOrAdd(
            gameId, _ => new ConcurrentDictionary<string, byte>());

        group.TryAdd(Context.ConnectionId, 0);
    }

    public async Task LeaveGame(Guid gameId)
    {
        var groupName = GroupName(gameId);

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

        if (_groups.TryGetValue(gameId, out var group))
        {
            group.TryRemove(Context.ConnectionId, out _);
        }

        await Clients
            .Group(groupName)
            .ClientDisconnected(new(Context.ConnectionId));
    }

    static List<string> GetGameConnections(Guid gameId)
    {
        return _groups.TryGetValue(gameId, out var group) 
            ? [.. group.Keys]
            : [];
    }

    public async Task<List<string>> NotifyPlayerMoved(PlayerMoved notification)
    {
        await Clients
            .Group(GroupName(notification.GameId))
            .PlayerMoved(notification);

        return GetGameConnections(notification.GameId);
    }

    public async Task AcknowledgePlayerMoved(AcknowledgePlayerMoved notification)
    {
        await Clients
            .Group(GroupName(notification.GameId))
            .AcknowledgePlayerMoved(
                new(notification.GameId,
                    notification.NotificationId,
                    Context.ConnectionId));
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

    static string GroupName(Guid gameId) => gameId.ToString();
}
