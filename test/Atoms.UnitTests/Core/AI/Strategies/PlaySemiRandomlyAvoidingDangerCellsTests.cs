namespace Atoms.UnitTests.Core.AI.Strategies;

public class PlaySemiRandomlyAvoidingDangerCellsTests : AIStrategyTestFixture
{
    [Test]
    public async Task ShouldChooseACellRandomly()
    {
        var game = ObjectMother.Game();
        var cell = game.Board[1, 1];

        SetNextMinMaxExpectations(1, 1);

        await Assert.That(Strategy.Choose(game)).IsEqualTo(cell);
    }

    [Test]
    public async Task ShouldNotChooseACellOccupiedByAnotherPlayer()
    {
        var game = ObjectMother.Game(active: 2, cells: [new(1, 1, 1, 1)]);

        SetNextMinMaxExpectations(1, 1, 2, 2);

        await Assert.That(Strategy.Choose(game)).IsEqualTo(game.Board[2, 2]);
    }

    [Test]
    public async Task ShouldNotChooseADangerCellWhenNumberOfTriesIsLessThan20()
    {
        var game = ObjectMother.Game(active: 2, cells: [new(1, 1, 1, 1)]);

        SetNextMinMaxExpectations(1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2,
                                  1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2,
                                  1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 2, 2);

        await Assert.That(Strategy.Choose(game)).IsEqualTo(game.Board[2, 2]);
    }

    [Test]
    public async Task ShouldOnlyChooseADangerCellWhenNumberOfTriesIsEqualTo20()
    {
        var game = ObjectMother.Game(active: 2, cells: [new(1, 1, 1, 1)]);

        SetNextMinMaxExpectations(1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2,
                                  1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2,
                                  1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2);

        await Assert.That(Strategy.Choose(game)).IsEqualTo(game.Board[1, 2]);
    }

    PlaySemiRandomlyAvoidingDangerCells Strategy => new(Rng);
}
