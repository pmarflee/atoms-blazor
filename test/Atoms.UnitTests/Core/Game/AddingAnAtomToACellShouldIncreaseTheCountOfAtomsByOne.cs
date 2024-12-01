namespace Atoms.UnitTests.Core.Game;

public class AddingAnAtomToACellShouldIncreaseTheCountOfAtomsByOne
{
    [Test, MethodDataSource(nameof(GetTestData))]
    public async Task Test(Atoms.Core.Entities.Game game, int row, int column)
    {
        var cell = game.Board[row, column];
        var atoms = cell.Atoms;

        game.PlaceAtom(cell);

        await Assert.That(cell.Atoms).IsEqualTo(atoms + 1);
    }

    public static IEnumerable<Func<(Atoms.Core.Entities.Game, int, int)>> GetTestData()
    {
        yield return () => (ObjectMother.Game(), 1, 1);
        yield return () => (
            ObjectMother.Game(cells: [ new(2, 1, 1, 1) ]),
            2, 1
        );
        yield return () => (
            ObjectMother.Game(cells: [ new(2, 2, 1, 1) ]),
            2, 2
        );
    }
}
