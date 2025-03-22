using Atoms.UseCases.PlayerMove;

namespace Atoms.UnitTests.UseCases.PlayerMove;

public class ShouldRecordLastUpdatedDateOnPlayerMoveSuccess : PlayerMoveAtomTestFixture
{
    [Test]
    public async Task Test()
    {
        var game = ObjectMother.Game();

        await Handler.Handle(
            new PlayerMoveRequest(game, game.Board[1, 1]),
            CancellationToken.None);

        await Assert.That(game.LastUpdatedDateUtc)
            .IsEqualTo(ObjectMother.NewLastUpdatedDateUtc);

        using var dbContext = await DbContextFactory.CreateDbContextAsync(CancellationToken.None);

        var gameDto = await dbContext.GetGameById(game.Id, CancellationToken.None);

        using var _ = Assert.Multiple();

        await Assert.That(gameDto).IsNotNull();
        await Assert.That(gameDto!.LastUpdatedDateUtc)
            .IsEqualTo(ObjectMother.NewLastUpdatedDateUtc);
    }
}
