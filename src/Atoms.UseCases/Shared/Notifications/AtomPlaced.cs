namespace Atoms.UseCases.Shared.Notifications;

public sealed class AtomPlaced(Game game) : GameStateChanged(game)
{
}
