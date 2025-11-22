using Atoms.Core.DTOs.Notifications.SignalR;

namespace Atoms.Core.Interfaces;

public interface IGameClient
{
    Task PlayerMoved(PlayerMoved notification);
    Task AcknowledgePlayerMoved(AcknowledgePlayerMoved notification);
    Task ClientDisconnected(ClientDisconnected notification);
    Task GameReloadRequired();
    Task PlayerJoined(PlayerJoined notification);
}
