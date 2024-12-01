using Atoms.UseCases.PlayerMove;

namespace Atoms.UnitTests.UseCases.PlayerMove;

public class OverloadingACellShouldTriggerAChainReaction : PlayerMoveAtomTestFixture
{
    [Test, MethodDataSource(nameof(GetTestData))]
    public async Task Test(Game game,
                           int row,
                           int column,
                           Game expected)
    {
        var cell = game.Board[row, column];

        await Handler.Handle(
            new PlayerMoveRequest(game, cell),
            CancellationToken.None);

        await Assert.That(game).IsEquivalentTo(expected);
    }

    public static IEnumerable<Func<(Game, int, int, Game)>> GetTestData()
    {
        yield return () => (
            ObjectMother.Game(cells: [new(1, 1, 1, 1)]),
            1, 1,
            ObjectMother.Game(
                active: 2,
                cells: [new(1, 2, 1, 1), new(2, 1, 1, 1)],
                move: 2)
        );

        yield return () => (
            ObjectMother.Game(cells: [new(1, 2, 1, 2)]),
            1, 2,
            ObjectMother.Game(
                active: 2,
                cells: [ new(1, 1, 1, 1), new(1, 3, 1, 1),
                         new(2, 2, 1, 1) ],
                move: 2)
        );

        yield return () => (
            ObjectMother.Game(cells: [new(2, 2, 1, 3)]),
            2, 2,
            ObjectMother.Game(
                active: 2,
                cells: [ new(1, 2, 1, 1), new(2, 1, 1, 1),
                         new(2, 3, 1, 1), new(3, 2, 1, 1) ],
                move: 2)
        );
    }
}
