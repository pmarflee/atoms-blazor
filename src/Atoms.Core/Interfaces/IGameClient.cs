namespace Atoms.Core.Interfaces;

public interface IGameClient
{
    Task PlayerMoved(int playerNumber);
}
