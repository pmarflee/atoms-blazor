using Atoms.UseCases.PlayerMove;

namespace Atoms.UnitTests.UseCases.PlayerMove;

public class ReturnSuccessResponseWhenAtomCanBePlaced : PlayerMoveAtomTestFixture
{
    [Test]
    public async Task Test()
    {
        var game = ObjectMother.Game();

        var response = await Handler.Handle(
            new PlayerMoveRequest(game, game.Board[1, 1]),
            CancellationToken.None);

        await Assert.That(response.IsSuccessful).IsTrue();
    }
}
