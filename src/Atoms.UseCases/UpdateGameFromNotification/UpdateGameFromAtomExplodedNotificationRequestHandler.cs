
namespace Atoms.UseCases.UpdateGameFromNotification;

public class UpdateGameFromAtomExplodedNotificationRequestHandler
    : UpdateCellFromNotificationRequestHandler<UpdateGameFromAtomExplodedNotificationRequest, AtomExploded>
{
    protected override void UpdateCell(
        Game game, AtomExploded notification, Game.Player player)
    {
        var cell = game.Board[notification.Row, notification.Column];

        cell.Explosion = notification.Explosion;

        if (notification.Explosion == ExplosionState.Before)
        {
            cell.Explode();
        }
    }
}
