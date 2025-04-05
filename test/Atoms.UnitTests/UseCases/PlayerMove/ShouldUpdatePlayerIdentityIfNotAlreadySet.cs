using Atoms.Core.ValueObjects;
using Atoms.UseCases.PlayerMove;

namespace Atoms.UnitTests.UseCases.PlayerMove;

public class ShouldUpdatePlayerIdentityIfNotAlreadySet : PlayerMoveAtomTestFixture
{
    [Test]
    public async Task TestWithLoggedInUser()
    {
        var game = ObjectMother.Game();

        await Handler.Handle(
            new PlayerMoveRequest(game, game.Board[1, 1], 
            userId: ObjectMother.UserId,
            username: ObjectMother.Username),
            CancellationToken.None);

        using var _ = Assert.Multiple();

        var player = game.Players[0];

        await Assert.That(player.UserId).IsEqualTo(ObjectMother.UserId);
        await Assert.That(player.Name).IsEqualTo(ObjectMother.Username);
        
        using var dbContext = await DbContextFactory.CreateDbContextAsync(
            CancellationToken.None);

        var gameDto = await dbContext.GetGameById(game.Id, CancellationToken.None);

        await Assert.That(gameDto).IsNotNull();

        var playerDto = gameDto!.Players.First(p => p.Number == 1);

        await Assert.That(playerDto.UserId).IsEqualTo(ObjectMother.UserId.Id);
        await Assert.That(playerDto.Name).IsEqualTo(ObjectMother.Username);
    }

    [Test]
    public async Task TestWithLocalStorage()
    {
        var game = ObjectMother.Game();

        await Handler.Handle(
            new PlayerMoveRequest(game, game.Board[1, 1], 
            username: ObjectMother.Username,
            localStorageId: ObjectMother.LocalStorageId),
            CancellationToken.None);

        using var _ = Assert.Multiple();

        var player = game.Players[0];

        await Assert.That(player.LocalStorageId).IsEqualTo(ObjectMother.LocalStorageId);
        await Assert.That(player.Name).IsEqualTo(ObjectMother.Username);
        
        using var dbContext = await DbContextFactory.CreateDbContextAsync(
            CancellationToken.None);

        var gameDto = await dbContext.GetGameById(game.Id, CancellationToken.None);

        await Assert.That(gameDto).IsNotNull();

        var playerDto = gameDto!.Players.First(p => p.Number == 1);

        await Assert.That(playerDto.LocalStorageId).IsEqualTo(ObjectMother.LocalStorageId.Value);
        await Assert.That(playerDto.Name).IsEqualTo(ObjectMother.Username);
    }

    [Test]
    public async Task TestWhenPlayerIdentityIsAlreadySet()
    {
        var userId = new UserId(Guid.NewGuid().ToString());
        var username = "Billy";

        List<Game.Player> players = [
            new(ObjectMother.Player1Id, 1, PlayerType.Human, 
                userId, username, localStorageId: ObjectMother.LocalStorageId),
            new(ObjectMother.Player2Id, 2, PlayerType.Human)];
        var game = ObjectMother.Game(players);

        await Handler.Handle(
            new PlayerMoveRequest(game, game.Board[1, 1]),
            CancellationToken.None);

        using var _ = Assert.Multiple();

        var player = game.Players[0];

        await Assert.That(player.UserId).IsEqualTo(userId);
        await Assert.That(player.Name).IsEqualTo(username);
        await Assert.That(player.LocalStorageId).IsEqualTo(ObjectMother.LocalStorageId);
        
        using var dbContext = await DbContextFactory.CreateDbContextAsync(
            CancellationToken.None);

        var gameDto = await dbContext.GetGameById(game.Id, CancellationToken.None);

        await Assert.That(gameDto).IsNotNull();

        var playerDto = gameDto!.Players.First(p => p.Number == 1);

        await Assert.That(playerDto.UserId).IsEqualTo(userId.Id);
        await Assert.That(playerDto.Name).IsEqualTo(username);
        await Assert.That(playerDto.LocalStorageId).IsEqualTo(ObjectMother.LocalStorageId.Value);
    }
}
