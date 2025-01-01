namespace Atoms.UnitTests.Core.AI.Strategies;

public class PlayRandomlyTests : AIStrategyTestFixture
{
    [Test]
    public async Task ShouldChooseACellRandomly()
    {
        var game = ObjectMother.Game();
        var cell = game.Board[1, 1];
        var strategy = Strategy(1, 1);

        await Assert.That(strategy.Choose(game)).IsEqualTo(cell);
    }

    [Test]
    public async Task ShouldNotChooseACellOccupiedByAnotherPlayer()
    {
        var game = ObjectMother.Game(
            active: 2,
            cells: [ new(1, 1, 1, 1) ]);

        var strategy = Strategy(1, 1, 2, 2);

        await Assert.That(strategy.Choose(game)).IsEqualTo(game.Board[2, 2]);
    }

    static PlayRandomly Strategy(params List<int> values) => new(new PlayRandomlyNumberGenerator(values));

    class PlayRandomlyNumberGenerator(params List<int> values) 
        : IRandomNumberGenerator
    {
        private int _index = 0;

        public int Next(int maxValue)
        {
            throw new NotImplementedException();
        }

        public int Next(int minValue, int maxValue)
        {
            return values[_index++];
        }

        public double NextDouble()
        {
            throw new NotImplementedException();
        }
    }
}
