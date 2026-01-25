using Atoms.Core.DTOs.Notifications.SignalR;

namespace Atoms.Core.Interfaces;

public interface INotificationService : IAsyncDisposable
{
    event Func<PlayerMoved, Task>? OnPlayerMoved;
    event Func<GameReloadRequired, Task>? OnGameReloadRequired;
    event Func<PlayerJoined, Task>? OnPlayerJoined;
    event Func<Rematch, Task>? OnRematch;

    Task NotifyPlayerMoved(PlayerMoved notification, CancellationToken cancellationToken = default);
    Task NotifyGameReloadRequired(GameReloadRequired notification,
                                  CancellationToken cancellationToken = default);
    Task NotifyPlayerJoined(PlayerJoined notification,
                            CancellationToken cancellationToken = default);
    Task Start(CancellationToken cancellationToken = default);
    Task JoinGame(Guid gameId, CancellationToken cancellationToken = default);
    Task<List<string>> GetOpponentConnections(Guid gameId, CancellationToken cancellationToken = default);
    Task NotifyRematch(Rematch notification, CancellationToken cancellationToken = default);
}