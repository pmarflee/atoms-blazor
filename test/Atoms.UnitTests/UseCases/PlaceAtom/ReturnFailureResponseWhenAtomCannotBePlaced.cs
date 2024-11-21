using Atoms.UseCases.PlaceAtom;

namespace Atoms.UnitTests.UseCases.PlaceAtom;

public class ReturnFailureResponseWhenAtomCannotBePlaced : PlaceAtomTestFixture
{
    [Test]
    public async Task Test()
    {
        var game = ObjectMother.Game(
            active: 2,
            cells: [ new(1, 1, 1, 1) ]);

        var response = await Handler.Handle(
            new PlaceAtomRequest(game, game.Board[1, 1]), 
            CancellationToken.None);

        await Assert.That(response.IsSuccessful).IsFalse();
    }
}
