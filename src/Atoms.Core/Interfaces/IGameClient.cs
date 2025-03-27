namespace Atoms.Core.Interfaces;

public interface IGameClient
{
    Task Notification(string message);
}
