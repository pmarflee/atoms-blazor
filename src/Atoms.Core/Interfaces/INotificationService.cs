using Atoms.Core.DTOs.Notifications.SignalR;

namespace Atoms.Core.Interfaces;

public interface INotificationService : IAsyncDisposable
{
    event Func<PlayerMoved, Task>? OnPlayerMoved;
    event Func<ClientDisconnected, Task>? OnClientDisconnected;
    event Func<GameReloadRequired, Task>? OnGameReloadRequired;
    event Func<PlayerJoined, Task>? OnPlayerJoined;

    Task<List<string>> NotifyPlayerMoved(PlayerMoved notification, CancellationToken cancellationToken = default);
    Task NotifyGameReloadRequired(GameReloadRequired notification,
                                  CancellationToken cancellationToken = default);
    Task NotifyPlayerJoined(PlayerJoined notification,
                            CancellationToken cancellationToken = default);
    Task Start(CancellationToken cancellationToken = default);
    Task JoinGame(Game game, CancellationToken cancellationToken = default);
    Task LeaveGame(Game game, CancellationToken cancellationToken = default);
    Task JoinGame(GameDTO game, CancellationToken cancellationToken = default);
    Task LeaveGame(GameDTO game, CancellationToken cancellationToken = default);
}