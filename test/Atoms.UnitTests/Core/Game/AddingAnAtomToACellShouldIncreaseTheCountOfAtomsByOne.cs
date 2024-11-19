using static Atoms.Core.Entities.Game;

namespace Atoms.UnitTests.Core.Game;

public class AddingAnAtomToACellShouldIncreaseTheCountOfAtomsByOne
{
    [Test, MethodDataSource(nameof(GetTestData))]
    public async Task Test(State state, int row, int column)
    {
        var game = new Atoms.Core.Entities.Game(state);
        var cell = game.Board[row, column];
        var atoms = cell.Atoms;

        game.PlaceAtom(cell);

        await Assert.That(cell.Atoms).IsEqualTo(atoms + 1);
    }

    public static IEnumerable<(State, int, int)> GetTestData()
    {
        yield return (ObjectMother.NewGameState, 1, 1);
        yield return (
            ObjectMother.NewGameState with 
            {
                Cells = 
                [
                    new State.Cell 
                    {
                        Row = 2, Column = 1, Atoms = 1, Player = 1 
                    }
                ]
            },
            2, 1
        );
        yield return (
            ObjectMother.NewGameState with 
            {
                Cells = 
                [
                    new State.Cell 
                    {
                        Row = 2, Column = 2, Atoms = 2, Player = 1 
                    }
                ]
            },
            2, 2
        );
    }
}
