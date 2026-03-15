using Atoms.UseCases.PlayerMove;

namespace Atoms.UnitTests.UseCases.PlayerMove;

public class ReturnGameDoesNotExistWhenGameMissing : PlayerMoveAtomTestFixture
{
    [Test]
    public async Task Test()
    {
        // Simulate the owner deleting the persisted game before the player makes a move.
        using (var dbContext = await DbContextFactory.CreateDbContextAsync())
        {
            var persisted = await dbContext.Games.FindAsync(ObjectMother.GameId);
            if (persisted != null)
            {
                dbContext.Games.Remove(persisted);
                await dbContext.SaveChangesAsync();
            }
        }

        // Create a local game entity that the player is attempting to play against (same id as deleted game)
        var game = ObjectMother.Game();

        LoggerExpectations.Setups
            .IsEnabled(Arg.Is(LogLevel.Debug))
            .ReturnValue(false);

        var handler = new PlayerMoveRequestHandler(
            DbContextFactory,
            BusExpectations.Instance(),
            LoggerExpectations.Instance());

        var result = await handler.Handle(
            new PlayerMoveRequest(game, new Position(1,1)),
            CancellationToken.None);

        await Assert.That(result.IsSuccessful).IsFalse();
        await Assert.That(result.Result).IsEqualTo(PlayerMoveResult.GameDoesNotExist);
        await Assert.That(result.AllowRetry).IsFalse();
    }
}
