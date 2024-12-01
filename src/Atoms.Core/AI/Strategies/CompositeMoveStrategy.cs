using Atoms.Core.Interfaces;

namespace Atoms.Core.AI.Strategies;

public class CompositeMoveStrategy(
    IPlayerStrategy first, IPlayerStrategy second) 
    : IPlayerStrategy
{
    public Game.GameBoard.Cell? Choose(Game game)
    {
        return first.Choose(game) ?? second.Choose(game);
    }
}
