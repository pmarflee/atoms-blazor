using static Atoms.Core.Entities.Game;

namespace Atoms.UnitTests.Core.Game;

public class OverloadingACellShouldCauseItToExplode
{
    [Test, MethodDataSource(nameof(GetTestData))]
    public async Task Test(State before, int row, int column, State after)
    {
        var game = Load(before);
        var cell = game.Board[row, column];

        game.PlaceAtom(cell);

        var result = game.Save();

        await Assert.That(after).IsEquivalentTo(result);
    }

    public static IEnumerable<(State, int, int, State)> GetTestData()
    {
        yield return (
            ObjectMother.NewGameState with
            {
                Cells = [ new(1, 1, 1, 1) ]
            },
            1, 1,
            ObjectMother.NewGameState with
            {
                Cells = [ new(1, 2, 1, 1), new(2, 1, 1, 1) ],
                Players =
                [
                    new(1, PlayerType.Human, false),
                    new(2, PlayerType.Human, true)
                ]
            }
        );

        yield return (
            ObjectMother.NewGameState with
            {
                Cells = [ new(1, 2, 1, 2) ]
            },
            1, 2,
            ObjectMother.NewGameState with
            {
                Cells = [ new(1, 1, 1, 1), new(1, 3, 1, 1), new(2, 2, 1, 1) ],
                Players =
                [
                    new(1, PlayerType.Human, false),
                    new(2, PlayerType.Human, true)
                ]
            }
        );

        yield return (
            ObjectMother.NewGameState with
            {
                Cells = [ new(2, 2, 1, 3) ]
            },
            2, 2,
            ObjectMother.NewGameState with
            {
                Cells = 
                [
                    new(1, 2, 1, 1), new(2, 1, 1, 1),
                    new(3, 2, 1, 1), new(2, 3, 1, 1) 
                ],
                Players =
                [
                    new(1, PlayerType.Human, false),
                    new(2, PlayerType.Human, true)
                ]
            }
        );
    }
}
