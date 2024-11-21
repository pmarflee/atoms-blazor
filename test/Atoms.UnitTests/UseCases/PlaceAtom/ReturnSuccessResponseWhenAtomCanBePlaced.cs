using Atoms.UseCases.PlaceAtom;
using Atoms.UseCases.Shared.Notifications;

namespace Atoms.UnitTests.UseCases.PlaceAtom;

public class ReturnSuccessResponseWhenAtomCanBePlaced : PlaceAtomTestFixture
{
    [Test]
    public async Task Test()
    {
        var game = ObjectMother.Game();

        var response = await Handler.Handle(
            new PlaceAtomRequest(game, game.Board[1, 1]), 
            CancellationToken.None);

        await Assert.That(response.IsSuccessful).IsTrue();
    }
}
