using GameEntity = Atoms.Core.Entities.Game;

namespace Atoms.UnitTests.Core.Entities.Game;

public class CreateOptionsForRematch
{
    [Test]
    public void ShouldThrowExceptionWhenGameIsNotFinished()
    {
        Assert.Throws<InvalidOperationException>(
            () => ObjectMother.GameDTO().CreateOptionsForRematch());
    }

    [Test, MethodDataSource(nameof(GetTestData))]
    public async Task TestCreateOptions(
        GameDTO game, bool hasSound)
    {
        var options = game.CreateOptionsForRematch(hasSound);

        using var _ = Assert.Multiple();

        await Assert.That(options.HasSound).IsEqualTo(hasSound);
        await Assert.That(options.Players.Count).IsEqualTo(game.Players.Count);
        await Assert.That(options.ColourScheme).IsEqualTo(game.ColourScheme);
        await Assert.That(options.AtomShape).IsEqualTo(game.AtomShape);
        await Assert.That(options.IsRematch).IsTrue();

        HashSet<GameMenuOptions.Player> foundPlayers = [];
        HashSet<PlayerDTO> unmatchedPlayers = [];

        foreach (var gamePlayer in game.Players)
        {
            var foundPlayer = options.Players
                .FirstOrDefault(op => 
                    op.Type == gamePlayer.PlayerTypeId &&
                    op.UserIdentity?.Id == gamePlayer.UserId &&
                    op.VisitorId == gamePlayer.VisitorId &&
                    !foundPlayers.Contains(op));

            if (foundPlayer is not null)
            {
                foundPlayers.Add(foundPlayer);
            }
            else
            {
                unmatchedPlayers.Add(gamePlayer);
            }
        }

        if (foundPlayers.Count < game.Players.Count)
        {
            Assert.Fail($"The following players were not matched: {string.Join(",", unmatchedPlayers.Select(p => p.Number))}");
        }
    }

    public static IEnumerable<Func<(GameDTO, bool)>> GetTestData()
    {
        yield return () =>
        (
            ObjectMother.GameDTO(
                board: ObjectMother.BoardDTO("2-1-1-2,1-2-1-1"),
                move: 4, round: 2,
                isActive: false),
                false
        );

        yield return () =>
        (
            ObjectMother.GameDTO(
                [
                    new PlayerDTO
                    {
                        Id = Guid.NewGuid(),
                        Number = 1,
                        PlayerTypeId = PlayerType.Human,
                        UserId = Guid.NewGuid().ToString(),
                        VisitorId = ObjectMother.VisitorId.Value
                    },
                    new PlayerDTO
                    {
                        Id = Guid.NewGuid(),
                        Number = 2,
                        PlayerTypeId = PlayerType.CPU_Easy
                    }
                ],
                4, 2,
                ObjectMother.BoardDTO("2-1-1-2,1-2-1-1"),
                isActive: false),
                false
        );
    }
}
