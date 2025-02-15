namespace Atoms.UnitTests.Core.AI.Strategies;

public class ChooseCornerCellTests : AIStrategyTestFixture
{
    [Test, MethodDataSource(nameof(FourCornerTestCases))]
    public async Task ShouldChooseCornerCellWhenRngIsBelowThreshold(
        double rnd, Game game, int row, int column)
    {
        RandomNumberGeneratorExpectations.Methods.NextDouble().ReturnValue(rnd);

        await Assert.That(Strategy.Choose(game)).IsEqualTo(game.Board[row, column]);
    }

    [Test]
    [MatrixDataSource]
    public async Task ShouldNotChooseCornerCellWhenRngIsAboveThreshold(
        [Matrix(0.8, 0.9, 0.99)] double rnd)
    {
        RandomNumberGeneratorExpectations.Methods.NextDouble().ReturnValue(rnd);

        await Assert.That(Strategy.Choose(ObjectMother.Game())).IsNull();
    }

    [Test]
    public async Task ShouldNotChooseCornerCellWhenAllCornerCellsAreTaken()
    {
        RandomNumberGeneratorExpectations.Methods.NextDouble().ReturnValue(0);

        var game = ObjectMother.Game(
                    cells:
                    [
                        new(1, 1, 1, 1),
                        new(1, ObjectMother.Columns, 1, 1),
                        new(ObjectMother.Rows, 1, 1, 1),
                        new(ObjectMother.Rows, ObjectMother.Columns, 1, 1)
                    ]);

        await Assert.That(Strategy.Choose(game)).IsNull();
    }

    [Test]
    public async Task ShouldNotChooseCornerCellsThatAreDangerCells()
    {
        RandomNumberGeneratorExpectations.Methods.NextDouble().ReturnValue(0);

        var game = ObjectMother.Game(cells: [ new(1, 2, 2, 2), ]);

        await Assert.That(Strategy.Choose(game))
            .IsEqualTo(game.Board[1, ObjectMother.Columns]);
    }

    public static IEnumerable<Func<(double, Game, int, int)>> FourCornerTestCases()
    {
        for (double nextDouble = 0; nextDouble < 0.8; nextDouble += 0.1f)
        {
            yield return () => (nextDouble, ObjectMother.Game(), 1, 1);
            yield return () => (
                nextDouble,
                ObjectMother.Game(
                    cells: [new(1, 1, 1, 1)]), 1, ObjectMother.Columns);
            yield return () => (
                nextDouble,
                ObjectMother.Game(
                    cells:
                    [
                        new(1, 1, 1, 1),
                        new(1, ObjectMother.Columns, 1, 1)
                    ]),
                    ObjectMother.Rows, 1);
            yield return () => (
                nextDouble,
                ObjectMother.Game(
                    cells:
                    [
                        new(1, 1, 1, 1),
                        new(1, ObjectMother.Columns, 1, 1),
                        new(ObjectMother.Rows, 1, 1, 1)
                    ]),
                    ObjectMother.Rows, ObjectMother.Columns);
        }
    }

    ChooseCornerCell Strategy => new(Rng);
}
