namespace Atoms.UnitTests.Core.Game;

public class ShouldAddAtomToCellWhenCellIsEmpty
{
    [Test, MethodDataSource(nameof(GetGameStates))]
    public async Task Test(Atoms.Core.Entities.Game game)
    {
        var cell = game.Board[1, 1];

        game.PlaceAtom(cell);

        await Assert.That(cell.Atoms).IsEqualTo(1);
        await Assert.That(cell.Player).IsEqualTo(1);
    }

    public static IEnumerable<Func<Atoms.Core.Entities.Game>> GetGameStates()
    {
        yield return () => ObjectMother.Game();
    }
}
