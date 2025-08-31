namespace Atoms.UseCases.Shared.Notifications;

public abstract record GameStateChanged(Guid GameId, Guid PlayerId) 
    : INotification
{
    public bool CanHandle(Game game) => game.Id == GameId;
}
