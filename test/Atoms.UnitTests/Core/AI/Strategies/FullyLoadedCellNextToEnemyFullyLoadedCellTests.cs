using Atoms.Core.AI.Strategies;
using static Atoms.Core.Entities.Game.GameBoard;

namespace Atoms.UnitTests.Core.AI.Strategies;

public class FullyLoadedCellNextToEnemyFullyLoadedCellTests : AIStrategyTestFixture
{
    [Test, MethodDataSource(nameof(TestCases))]
    public async Task TestStrategy(Game game, Cell? expected)
    {
        var strategy = new FullyLoadedCellNextToEnemyFullyLoadedCell();

        await Assert.That(strategy.Choose(game)).IsEqualTo(expected);
    }

    public static IEnumerable<Func<(Game, Cell?)>> TestCases()
    {
        static Func<(Game, Cell?)> CreateTestCase(
            List<CellState> cells, 
            (int Row, int Column)? choose,
            int? active = 1)
        {
            var game = ObjectMother.Game(active: active, cells: cells);
            var expected = choose.HasValue
                ? game.Board[choose.Value.Row, choose.Value.Column]
                : null;

            return () => (game, expected);
        }

        yield return CreateTestCase([], null);

        yield return CreateTestCase(
            [ new(1, 1, 2, 1), new(1, 2, 1, 2) ],
            (1, 2));

        yield return CreateTestCase(
            [ new(2, 2, 1, 3), new(3, 2, 2, 3) ],
            (3, 2), 2);

        yield return CreateTestCase(
            [ new(2, 2, 1, 2), new(3, 2, 2, 3) ],
            null, 2);

        yield return CreateTestCase(
            [ new(2, 2, 1, 3), new(3, 2, 2, 2) ],
            null, 2);
    }
}
