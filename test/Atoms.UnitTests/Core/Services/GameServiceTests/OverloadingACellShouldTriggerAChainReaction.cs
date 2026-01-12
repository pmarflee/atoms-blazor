using Atoms.Core.Services;

namespace Atoms.UnitTests.Core.Services.GameServiceTests;

public class OverloadingACellShouldTriggerAChainReaction
{
    [Test, MethodDataSource(nameof(GetTestData))]
    public async Task Test(Game game,
                           int row,
                           int column,
                           Game expected)
    {
        var service = new GameMoveService();
        var cell = game.Board[row, column];

        await service.PlayMove(game, cell);

        await Assert.That(game.Board.Cells).IsEquivalentTo(expected.Board.Cells);
        await Assert.That(game.ActivePlayer.Number).IsEquivalentTo(expected.ActivePlayer.Number);
        await Assert.That(game.Move).IsEquivalentTo(expected.Move);
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
