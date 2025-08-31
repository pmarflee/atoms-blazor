namespace Atoms.UseCases.UpdateGameFromNotification;

public class UpdateGameFromAtomPlacedNotificationRequestHandler 
    : UpdateCellFromNotificationRequestHandler<UpdateGameFromAtomPlacedNotificationRequest, AtomPlaced>
{
    protected override void UpdateCell(
        Game game, AtomPlaced notification, Game.Player player)
    {
        game.PlaceAtom(notification.Row, notification.Column, player);
    }
}
