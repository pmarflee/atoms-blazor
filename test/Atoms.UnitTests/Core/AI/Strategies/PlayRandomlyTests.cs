namespace Atoms.UnitTests.Core.AI.Strategies;

public class PlayRandomlyTests : AIStrategyTestFixture
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
        var game = ObjectMother.Game(
            active: 2,
            cells: [ new(1, 1, 1, 1) ]);

        SetNextMinMaxExpectations(1, 1, 2, 2);

        await Assert.That(Strategy.Choose(game)).IsEqualTo(game.Board[2, 2]);
    }

    PlayRandomly Strategy => new(Rng);
}
