using Atoms.UseCases.PlayerMove;

namespace Atoms.UnitTests.UseCases.PlayerMove;

public class ReturnFailureResponseWhenAtomCannotBePlaced : PlayerMoveAtomTestFixture
{
    [Test]
    public async Task Test()
    {
        var game = ObjectMother.Game(
            active: 2,
            cells: [new(1, 1, 1, 1)]);

        LoggerExpectations.Setups
            .IsEnabled(Arg.Is(LogLevel.Debug))
            .ReturnValue(false);

        var handler = new PlayerMoveRequestHandler(
            DbContextFactory, 
            BusExpectations.Instance(), 
            LoggerExpectations.Instance());

        var response = await handler.Handle(
            new PlayerMoveRequest(game, game.Board[1, 1]),
            CancellationToken.None);

        await Assert.That(response.IsSuccessful).IsFalse();
    }
}
