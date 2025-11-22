using Atoms.UseCases.PlayerMove;
using Atoms.UseCases.PlayerMove.Rebus;

namespace Atoms.UnitTests.UseCases.PlayerMove;

public class PostMessageToBusAndReturnSuccessResponseWhenAtomCanBePlaced : PlayerMoveAtomTestFixture
{
    [Test]
    public async Task Test()
    {
        var game = ObjectMother.Game();
        var cell = game.Board[1, 1];

        BusExpectations.Methods
            .Send(Arg.Validate<object>(
                x =>
                {
                    if (x is not PlayerMoveMessage msg) return false;

                    return msg.GameId == game.Id
                        && msg.Row == cell.Row
                        && msg.Column == cell.Column;
                }))
            .ReturnValue(Task.CompletedTask);

        var handler = new PlayerMoveRequestHandler(
            DbContextFactory, BusExpectations.Instance());

        var response = await handler.Handle(
            new PlayerMoveRequest(game, cell),
            CancellationToken.None);

        await Assert.That(response.IsSuccessful).IsTrue();
    }
}
