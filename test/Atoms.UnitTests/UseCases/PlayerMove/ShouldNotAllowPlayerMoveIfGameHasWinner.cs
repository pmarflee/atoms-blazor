using Atoms.UseCases.PlayerMove;

namespace Atoms.UnitTests.UseCases.PlayerMove;

public class ShouldNotAllowPlayerMoveIfGameHasWinner : PlayerMoveAtomTestFixture
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

        var result = await Handler.Handle(
            new PlayerMoveRequest(game, game.Board[1, 1]),
            CancellationToken.None);

        using var _ = Assert.Multiple();

        await Assert.That(result.IsSuccessful).IsFalse();
        await Assert.That(result.Result)
            .IsEqualTo(PlayerMoveResult.GameHasWinner);
    }

    protected override GameDTO GameState =>
        ObjectMother.GameDTO(
        [
            new PlayerDTO
            {
                Id = ObjectMother.Player1Id,
                Number = 1,
                PlayerTypeId = PlayerType.Human,
                IsWinner = true
            },
            new PlayerDTO 
            { 
                Id = ObjectMother.Player2Id,
                Number = 2,
                PlayerTypeId = PlayerType.CPU_Easy,
                IsWinner = false 
            }
        ],
        4, 2,
        new BoardDTO { Data = "1-2-1-1,2-1-1-2" });
}
