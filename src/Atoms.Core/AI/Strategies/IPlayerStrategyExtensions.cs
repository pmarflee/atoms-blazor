using Atoms.Core.Interfaces;

namespace Atoms.Core.AI.Strategies;

public static class IPlayerStrategyExtensions
{
    public static IPlayerStrategy Or(this IPlayerStrategy strategy,
                                              IPlayerStrategy other)
    {
        return new CompositeMoveStrategy(strategy, other);
    }
}
