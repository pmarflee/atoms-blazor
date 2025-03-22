using Atoms.UseCases.PlayerMove;

namespace Atoms.UnitTests.UseCases.PlayerMove;

public class ShouldNotAllowPlayerMoveIfGameStateHasChanged : PlayerMoveAtomTestFixture
{
    [Test]
    public async Task Test()
    {
        using var dbContext = await DbContextFactory.CreateDbContextAsync(CancellationToken.None);

        var gameDto = await dbContext.GetGameById(
            ObjectMother.GameId, CancellationToken.None);

        var game = await gameDto!.ToEntity(
            ObjectMother.CreateRng, 
            ObjectMother.CreatePlayerStrategy, 
            ObjectMother.GetUserById);

        gameDto.LastUpdatedDateUtc = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(CancellationToken.None);

        var result = await Handler.Handle(
            new PlayerMoveRequest(game, game.Board[1, 1]),
            CancellationToken.None);

        using var _ = Assert.Multiple();

        await Assert.That(result.IsSuccessful).IsFalse();
        await Assert.That(result.Result)
            .IsEqualTo(PlayerMoveResult.GameStateHasChanged);
    }
}
