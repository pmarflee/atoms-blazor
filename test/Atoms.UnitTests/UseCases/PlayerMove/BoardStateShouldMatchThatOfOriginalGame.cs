using Atoms.Core.Test;
using Atoms.UseCases.PlayerMove;

namespace Atoms.UnitTests.UseCases.PlayerMove;

public class BoardStateShouldMatchThatOfOriginalGame : PlayerMoveAtomTestFixture
{
    [Test, MethodDataSource(nameof(GetTestData))]
    public async Task Test(int numberOfMoves,
                           Game.GameBoard expected,
                           int? winner)
    {
        var game = ObjectMother.Game();
        var moves = new Moves().Take(numberOfMoves);

        foreach (var (row, column) in moves)
        {
            await Handler.Handle(
                new PlayerMoveRequest(game, game.Board[row, column]),
                CancellationToken.None);
        }

        using var _ = Assert.Multiple();

        await Assert.That(game.Board).IsEquivalentTo(expected);
        await Assert.That(game.Winner?.Number).IsEqualTo(winner);
    }

    public static IEnumerable<Func<(int, Game.GameBoard, int?)>> GetTestData()
    {
        yield return () => (
            84,
            ObjectMother.Game(
                cells:
                [
                    new(1, 1, 1, 1),
                    new(1, 2, 2, 2),
                    new(1, 3, 2, 1),
                    new(1, 4, 1, 2),
                    new(1, 6, 1, 2),
                    new(1, 7, 1, 2),
                    new(1, 8, 1, 2),
                    new(1, 9, 1, 2),
                    new(1, 10, 1, 1),
                    new(2, 2, 2, 1),
                    new(2, 3, 1, 2),
                    new(2, 5, 1, 2),
                    new(2, 6, 2, 3),
                    new(2, 7, 2, 1),
                    new(2, 8, 1, 3),
                    new(2, 9, 1, 1),
                    new(2, 10, 2, 2),
                    new(3, 2, 2, 1),
                    new(3, 3, 2, 2),
                    new(3, 4, 1, 2),
                    new(3, 5, 2, 3),
                    new(3, 7, 2, 1),
                    new(3, 8, 2, 3),
                    new(3, 9, 2, 2),
                    new(4, 1, 1, 1),
                    new(4, 2, 2, 1),
                    new(4, 4, 2, 3),
                    new(4, 5, 2, 3),
                    new(4, 6, 2, 2),
                    new(4, 7, 2, 2),
                    new(4, 8, 2, 2),
                    new(4, 9, 2, 1),
                    new(4, 10, 2, 2),
                    new(5, 3, 2, 1),
                    new(5, 4, 2, 3),
                    new(5, 6, 2, 3),
                    new(5, 7, 2, 3),
                    new(5, 8, 2, 1),
                    new(5, 9, 2, 3),
                    new(5, 10, 2, 2),
                    new(6, 5, 2, 1),
                    new(6, 6, 2, 2),
                    new(6, 7, 2, 1),
                    new(6, 8, 2, 2),
                    new(6, 9, 2, 1)
                ]).Board,
                null);

        yield return () => (
            107,
            ObjectMother.Game(
                cells:
                [
                    new(1, 1, 1, 1),
                        new(1, 2, 1, 3),
                        new(1, 4, 1, 2),
                        new(1, 5, 1, 2),
                        new(1, 6, 1, 1),
                        new(1, 7, 1, 2),
                        new(1, 9, 1, 2),
                        new(2, 1, 1, 4),
                        new(2, 2, 1, 2),
                        new(2, 3, 1, 2),
                        new(2, 4, 1, 4),
                        new(2, 5, 1, 2),
                        new(2, 6, 1, 2),
                        new(2, 7, 1, 1),
                        new(2, 8, 1, 3),
                        new(2, 9, 1, 3),
                        new(2, 10, 1, 2),
                        new(3, 1, 1, 1),
                        new(3, 2, 1, 3),
                        new(3, 4, 1, 1),
                        new(3, 5, 1, 2),
                        new(3, 6, 1, 3),
                        new(3, 7, 1, 3),
                        new(3, 9, 1, 1),
                        new(3, 10, 1, 2),
                        new(4, 1, 1, 1),
                        new(4, 2, 1, 3),
                        new(4, 3, 1, 2),
                        new(4, 4, 1, 3),
                        new(4, 5, 1, 3),
                        new(4, 6, 1, 1),
                        new(4, 7, 1, 2),
                        new(4, 8, 1, 2),
                        new(4, 9, 1, 3),
                        new(4, 10, 1, 2),
                        new(5, 1, 1, 1),
                        new(5, 2, 1, 3),
                        new(5, 3, 1, 3),
                        new(5, 4, 1, 2),
                        new(5, 5, 1, 2),
                        new(5, 7, 1, 1),
                        new(5, 8, 1, 3),
                        new(5, 9, 1, 3),
                        new(5, 10, 1, 2),
                        new(6, 2, 1, 2),
                        new(6, 4, 1, 2),
                        new(6, 6, 1, 2),
                        new(6, 7, 1, 2),
                        new(6, 8, 1, 2),
                        new(6, 9, 1, 1)
                ]).Board,
                1);
    }
}
