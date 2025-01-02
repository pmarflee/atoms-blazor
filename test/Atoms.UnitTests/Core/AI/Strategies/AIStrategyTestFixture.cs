namespace Atoms.UnitTests.Core.AI.Strategies;

public abstract class AIStrategyTestFixture
{
    internal IRandomNumberGeneratorCreateExpectations RandomNumberGeneratorExpectations { get; private set; } = default!;
    protected IRandomNumberGenerator Rng => RandomNumberGeneratorExpectations.Instance();
    
    [Before(Test)]
    public Task SetupBase()
    {
        RandomNumberGeneratorExpectations = new IRandomNumberGeneratorCreateExpectations();

        return Task.CompletedTask;
    }

    protected static Func<(Game, Cell?)> CreateTestCase(
        List<CellState> cells, 
        (int Row, int Column)? choose,
        int? active = 1)
    {
        return () => CreateTestCaseValues(cells, choose, active);
    }

    protected static (Game, Cell?) CreateTestCaseValues(
        List<CellState> cells, 
        (int Row, int Column)? choose,
        int? active = 1)
    {
        var game = ObjectMother.Game(active: active, cells: cells);
        var expected = choose.HasValue
            ? game.Board[choose.Value.Row, choose.Value.Column]
            : null;

        return (game, expected);
    }

    protected void SetNextMinMaxExpectations(params int[] values)
    {
        int i = 0;

        RandomNumberGeneratorExpectations.Methods.Next(
            Arg.Any<int>(), Arg.Any<int>())
            .Callback((_, _) => values[i++]);
    }
}
