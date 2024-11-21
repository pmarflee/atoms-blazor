namespace Atoms.UseCases.Shared.Notifications;

public class GameStateChanged(Game game) : INotification
{
    public Game Game { get; } = game;
}
