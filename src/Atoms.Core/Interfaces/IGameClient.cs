using Atoms.Core.DTOs.Notifications.SignalR;

namespace Atoms.Core.Interfaces;

public interface IGameClient
{
    Task PlayerMoved(PlayerMoved notification);
    Task GameReloadRequired();
    Task PlayerJoined(PlayerJoined notification);
    Task Rematch(Rematch notification);
}
