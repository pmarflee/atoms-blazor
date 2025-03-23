using Microsoft.AspNetCore.SignalR;

namespace Atoms.Web.Hubs;

public class GameHub : Hub<IGameClient>
{
    public async Task AddPlayer(Guid gameId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, GetGroupName(gameId));
    }

    public async Task SendNotification(Guid gameId, int playerNumber)
    {
        await Clients
            .GroupExcept(GetGroupName(gameId), Context.ConnectionId)
            .PlayerMoved(playerNumber);
    }

    static string GetGroupName(Guid gameId) => gameId.ToString();
}
