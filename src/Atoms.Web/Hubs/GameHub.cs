using Microsoft.AspNetCore.SignalR;

namespace Atoms.Web.Hubs;

public class GameHub : Hub<IGameClient>
{
    public const string HubUrl = "/gamehub";

    public async Task AddPlayer(Guid gameId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, GetGroupName(gameId));
    }

    public async Task Notify(Guid gameId, string message)
    {
        await Clients
            .GroupExcept(GetGroupName(gameId), Context.ConnectionId)
            .Notification(message);
    }

    static string GetGroupName(Guid gameId) => gameId.ToString();
}
