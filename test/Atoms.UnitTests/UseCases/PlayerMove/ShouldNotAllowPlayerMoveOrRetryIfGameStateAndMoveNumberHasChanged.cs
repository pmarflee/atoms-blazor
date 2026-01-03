using Atoms.UseCases.PlayerMove;

namespace Atoms.UnitTests.UseCases.PlayerMove;

public class ShouldNotAllowPlayerMoveOrRetryIfGameStateAndMoveNumberHasChanged : PlayerMoveAtomTestFixture
{
    [Test]
    public async Task Test()
    {
        using var dbContext = await DbContextFactory.CreateDbContextAsync(CancellationToken.None);

        var game = ObjectMother.Game();

        await dbContext.SaveChangesAsync(CancellationToken.None);

        LoggerExpectations.Setups
            .IsEnabled(Arg.Is(LogLevel.Debug))
            .ReturnValue(false);

        var handler = new PlayerMoveRequestHandler(
            DbContextFactory, 
            BusExpectations.Instance(), 
            LoggerExpectations.Instance());

        var result = await handler.Handle(
            new PlayerMoveRequest(game, new(1, 1)),
            CancellationToken.None);

        using var _ = Assert.Multiple();

        await Assert.That(result.IsSuccessful).IsFalse();
        await Assert.That(result.Result)
            .IsEqualTo(PlayerMoveResult.GameStateHasChanged);
        await Assert.That(result.AllowRetry).IsFalse();
    }

    protected override GameDTO GameState =>
        ObjectMother.GameDTO(
            move: 2,
            board: ObjectMother.BoardDTO("1-1-1-1"),
            lastUpdatedDateUtc: ObjectMother.LastUpdatedDateUtc.AddMinutes(5)
        );
}
