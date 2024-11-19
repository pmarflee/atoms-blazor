namespace Atoms.Core.Interfaces;

public interface IGameFactory
{
    Game Create(GameMenuOptions menuDto);
    Game Create(Game.State state);
}
