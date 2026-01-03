using Atoms.Core.ValueObjects;
using Atoms.UseCases.PlayerMove;
using Atoms.UseCases.PlayerMove.Rebus;

namespace Atoms.UnitTests.UseCases.PlayerMove;

public class PostMessageToBusAndReturnSuccessResponseWhenAtomCanBePlaced : PlayerMoveAtomTestFixture
{
    [Test]
    public async Task Test()
    {
        var game = ObjectMother.Game();
        var position = new Position(1, 1);

        BusExpectations.Setups
            .Send(Arg.Validate<object>(
                x =>
                {
                    if (x is not PlayerMoveMessage msg) return false;

                    return msg.GameId == game.Id
                        && msg.Row == position.Row
                        && msg.Column == position.Column;
                }))
            .ReturnValue(Task.CompletedTask);

        LoggerExpectations.Setups
            .IsEnabled(Arg.Is(LogLevel.Debug))
            .ReturnValue(false);

        var handler = new PlayerMoveRequestHandler(
            DbContextFactory, 
            BusExpectations.Instance(), 
            LoggerExpectations.Instance());

        var response = await handler.Handle(
            new PlayerMoveRequest(game, position),
            CancellationToken.None);

        await Assert.That(response.IsSuccessful).IsTrue();
    }
}
