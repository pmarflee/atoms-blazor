using Atoms.Core.Identity;
using Atoms.Core.ValueObjects;

namespace Atoms.UnitTests.Core.Entities.Game;

public class CanPlayMoveTests
{
    [Test]
    public async Task ShouldNotBeAbleToPlayMoveWhenGameHasWinner()
    {
        var game = ObjectMother.Game(cells: [new(1, 1, 1, 1)], move: 3, round: 2);

        await Assert.That(
            game.CanPlayMove(ObjectMother.UserId, ObjectMother.LocalStorageId))
            .IsFalse();
    }

    [Test]
    public async Task ShouldNotBeAbleToPlayMoveWhenActivePlayerIsNotHuman()
    {
        var game = ObjectMother.Game(
            [ObjectMother.CreateHumanPlayer(ObjectMother.Player1Id, 1),
             ObjectMother.CreateCPUPlayer(ObjectMother.Player2Id, 2)],
            2, [new(1, 1, 1, 1)], move: 2, round: 1);

        await Assert.That(
            game.CanPlayMove(ObjectMother.UserId, ObjectMother.LocalStorageId))
            .IsFalse();
    }

    [Test]
    public async Task ShouldBeAbleToPlayMoveWhenUserIdMatchesThatOfTheActivePlayer()
    {
        var player1 = ObjectMother.CreateHumanPlayer(
            ObjectMother.Player1Id, 1,
            ObjectMother.CreateApplicationUser(ObjectMother.UserId));
        var player2 = ObjectMother.CreateCPUPlayer(ObjectMother.Player2Id, 2);

        var game = ObjectMother.Game(
            [player1, player2],
            userId: UserId.FromGuid(Guid.NewGuid()),
            localStorageId: new StorageId(Guid.NewGuid()));

        await Assert.That(
            game.CanPlayMove(ObjectMother.UserId, ObjectMother.LocalStorageId))
            .IsTrue();
    }

    [Test]
    public async Task ShouldBeAbleToPlayMoveWhenLocalStorageIdMatchesThatOfTheActivePlayer()
    {
        var player1 = ObjectMother.CreateHumanPlayer(
            ObjectMother.Player1Id, 1,
            localStorageId: ObjectMother.LocalStorageId);
        var player2 = ObjectMother.CreateCPUPlayer(ObjectMother.Player2Id, 2);

        var game = ObjectMother.Game(
            [player1, player2],
            userId: UserId.FromGuid(Guid.NewGuid()),
            localStorageId: new StorageId(Guid.NewGuid()));

        await Assert.That(
            game.CanPlayMove(ObjectMother.UserId, ObjectMother.LocalStorageId))
            .IsTrue();
    }

    [Test]
    public async Task ShouldBeAbleToPlayMoveWhenActivePlayerDoesNotHaveAUserIdOrLocalStorageIdAndUserIdMatchesThatOfTheGame()
    {
        var player1 = ObjectMother.CreateHumanPlayer(ObjectMother.Player1Id, 1);
        var player2 = ObjectMother.CreateCPUPlayer(ObjectMother.Player2Id, 2);

        var game = ObjectMother.Game(
            [player1, player2],
            localStorageId: new StorageId(Guid.NewGuid()));

        await Assert.That(
            game.CanPlayMove(ObjectMother.UserId, ObjectMother.LocalStorageId))
            .IsTrue();
    }

    [Test]
    public async Task ShouldBeAbleToPlayMoveWhenActivePlayerDoesNotHaveAUserIdOrLocalStorageIdAndLocalStorageIdMatchesThatOfTheGame()
    {
        var player1 = ObjectMother.CreateHumanPlayer(ObjectMother.Player1Id, 1);
        var player2 = ObjectMother.CreateCPUPlayer(ObjectMother.Player2Id, 2);

        var game = ObjectMother.Game(
            [player1, player2],
            localStorageId: new StorageId(Guid.NewGuid()));

        await Assert.That(
            game.CanPlayMove(ObjectMother.UserId, ObjectMother.LocalStorageId))
            .IsTrue();
    }

    [Test]
    public async Task ShouldNotBeAbleToPlayMoveWhenActivePlayerDoesNotHaveAUserIdOrLocalStorageIdAndNeitherUserIdOrLocalStorageIdMatchesThatOfTheGame()
    {
        var player1 = ObjectMother.CreateHumanPlayer(ObjectMother.Player1Id, 1);
        var player2 = ObjectMother.CreateCPUPlayer(ObjectMother.Player2Id, 2);

        var game = ObjectMother.Game(
            [player1, player2],
            userId: UserId.FromGuid(Guid.NewGuid()),
            localStorageId: new StorageId(Guid.NewGuid()));

        await Assert.That(
            game.CanPlayMove(ObjectMother.UserId, ObjectMother.LocalStorageId))
            .IsFalse();
    }

    [Test]
    public async Task ShouldNotBeAbleToPlayMoveWhenUserIsLoggedInAndActivePlayerIsAUserWhoIsNotTheSameUser()
    {
        var player1 = ObjectMother.CreateHumanPlayer(
            ObjectMother.Player1Id, 1, 
            new ApplicationUser { Id = ObjectMother.UserId.Id });
        var player2 = ObjectMother.CreateHumanPlayer(
            ObjectMother.Player2Id, 2, 
            new ApplicationUser { Id = Guid.NewGuid().ToString() });

        var game = ObjectMother.Game(
            [player1, player2], 2,
            userId: ObjectMother.UserId);

        await Assert.That(
            game.CanPlayMove(ObjectMother.UserId, ObjectMother.LocalStorageId))
            .IsFalse();
    }

    [Test]
    public async Task ShouldNotBeAbleToPlayMoveWhenUserHasALocalStorageIdAndTheActivePlayerHasALocalStorageIdThatDoesNotMatch()
    {
        var player1 = ObjectMother.CreateHumanPlayer(
            ObjectMother.Player1Id, 1,
            localStorageId: ObjectMother.LocalStorageId);
        var player2 = ObjectMother.CreateHumanPlayer(
            ObjectMother.Player2Id, 2, 
            localStorageId: new StorageId(Guid.NewGuid()));

        var game = ObjectMother.Game(
            [player1, player2], 2);

        await Assert.That(
            game.CanPlayMove(null, ObjectMother.LocalStorageId))
            .IsFalse();
    }
}
