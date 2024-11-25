namespace Atoms.UseCases.Shared.Notifications;

public abstract class GameStateChanged(Game game) : INotification
{
    public Game Game { get; } = game;
}
