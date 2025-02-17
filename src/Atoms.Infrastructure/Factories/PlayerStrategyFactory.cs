using Atoms.Core.AI.Strategies;

namespace Atoms.Infrastructure.Factories;

public static class PlayerStrategyFactory
{
    public static IPlayerStrategy? Create(PlayerType playerType,
                                          IRandomNumberGenerator rng) =>
        playerType switch
        {
            PlayerType.CPU_Easy => new PlayRandomly(rng),
            PlayerType.CPU_Medium =>
                new FullyLoadedCellNextToEnemyFullyLoadedCell()
                .Or(new PlaySemiRandomlyAvoidingDangerCells(rng)),
            PlayerType.CPU_Hard =>
                new FullyLoadedCellNextToEnemyFullyLoadedCell()
                    .Or(new GainAdvantageOverNeighbouringCell(rng))
                    .Or(new ChooseCornerCell(rng))
                    .Or(new PlaySemiRandomlyAvoidingDangerCells(rng)),
            _ => null
        };
}
