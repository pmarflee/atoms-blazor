
using Atoms.Core.State;

namespace Atoms.UseCases.UpdateGameFromPlayerMoveNotification;

public class UpdateGameFromPlayerMoveNotificationRequestHandler(
    IGameService gameService, GameStateContainer stateContainer)
    : IRequestHandler<UpdateGameFromPlayerMoveNotificationRequest>
{
    public async Task Handle(UpdateGameFromPlayerMoveNotificationRequest request,
                             CancellationToken cancellationToken)
    {
        var game = request.Game;
        var notification = request.Notification;
        var cell = notification.Row.HasValue && notification.Column.HasValue
            ? game.Board[notification.Row.Value, notification.Column.Value] 
            : null;

        await gameService.PlayAllMoves(
            game, cell, stateContainer.NotifyGameStateChanged);
    }
}
