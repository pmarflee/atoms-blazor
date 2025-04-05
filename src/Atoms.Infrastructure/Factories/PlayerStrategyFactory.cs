using Atoms.Core.AI.Strategies;

namespace Atoms.Infrastructure.Factories;

public static class PlayerStrategyFactory
{
    public static IPlayerStrategy? Create(PlayerType playerType,
                                          IRandomNumberGenerator rng)
    {
        IPlayerStrategy? strategy = null;

        playerType
            .When(PlayerType.CPU_Easy).Then(
                () => strategy = new PlayRandomly(rng))
            .When(PlayerType.CPU_Medium).Then(
                () => strategy = new FullyLoadedCellNextToEnemyFullyLoadedCell()
                                 .Or(new PlaySemiRandomlyAvoidingDangerCells(rng)))
            .When(PlayerType.CPU_Hard).Then(
                () => strategy = new FullyLoadedCellNextToEnemyFullyLoadedCell()
                                 .Or(new GainAdvantageOverNeighbouringCell(rng))
                                 .Or(new ChooseCornerCell(rng))
                                 .Or(new PlaySemiRandomlyAvoidingDangerCells(rng)));

        return strategy;
    }
}
