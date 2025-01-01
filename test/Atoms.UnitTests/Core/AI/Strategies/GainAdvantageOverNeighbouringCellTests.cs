namespace Atoms.UnitTests.Core.AI.Strategies;

public class GainAdvantageOverNeighbouringCellTests : AIStrategyTestFixture
{
    [Test]
    public async Task ShouldNotChooseCornerCellWhenRngIsAboveThreshold(
        [Matrix(0.9, 0.95, 0.99)] double rnd)
    {
        RandomNumberGeneratorExpectations.Methods.NextDouble().ReturnValue(rnd);

        await Assert.That(Strategy.Choose(ObjectMother.Game())).IsNull();
    }

    [Test, MethodDataSource(nameof(TestCases))]
    public async Task ShouldChooseCornerCellWhenRngIsBelowThreshold(
        Game game, Cell? cell, double nextDouble, int nextInt)
    {
        RandomNumberGeneratorExpectations.Methods.NextDouble().ReturnValue(nextDouble);
        RandomNumberGeneratorExpectations.Methods.Next(Arg.Any<int>()).ReturnValue(nextInt);

        await Assert.That(Strategy.Choose(game)).IsEqualTo(cell);
    }

    public static IEnumerable<Func<(Game, Cell?, double, int)>> TestCases()
    {
        for (double nextDouble = 0; nextDouble < 0.9; nextDouble += 0.1f)
        {
            yield return CreateTestCase([], null, nextDouble);
            yield return CreateTestCase(
                [ new(1, 1, 2, 1), new(1, 2, 1, 2) ],
                (1, 2),
                nextDouble);
            yield return CreateTestCase(
                [ new(1, 1, 2, 1), new(1, 2, 1, 1) ],
                null,
                nextDouble);
            yield return CreateTestCase(
                [ new(1, 1, 2, 1), new(1, 2, 1, 2), new(2, 1, 1, 2) ],
                (1, 2),
                nextDouble);
            yield return CreateTestCase(
                [ new(1, 1, 2, 1), new(1, 2, 1, 2), new(2, 1, 1, 2) ],
                (2, 1),
                nextDouble, 1);
            yield return CreateTestCase(
                [new(2, 2, 2, 1), new(2, 1, 1, 1)],
                (2, 1),
                nextDouble);
            yield return CreateTestCase(
                [new(2, 2, 2, 2), new(2, 1, 1, 1)],
                (2, 1),
                nextDouble);
            yield return CreateTestCase(
                [new(2, 2, 2, 2), new(2, 1, 1, 1), new(2, 3, 1, 2)],
                (2, 1),
                nextDouble);
            yield return CreateTestCase(
                [new(2, 2, 2, 2), new(2, 1, 1, 1), new(2, 3, 1, 2)],
                (2, 3),
                nextDouble, 1);
            yield return CreateTestCase(
                [new(2, 2, 2, 3), new(2, 1, 1, 1), new(2, 3, 1, 3)],
                (2, 3),
                nextDouble);
        }
    }

    static Func<(Game, Cell?, double, int)> CreateTestCase(
        List<CellState> cells, 
        (int Row, int Column)? choose,
        double nextDouble,
        int nextInt = 0,
        int? active = 1)
    {
        var (game, expected) = CreateTestCaseValues(cells, choose, active);

        return () => (game, expected, nextDouble, nextInt);
    }

    GainAdvantageOverNeighbouringCell Strategy => new(Rng);
}
