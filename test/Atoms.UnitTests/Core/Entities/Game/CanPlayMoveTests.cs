using Atoms.Core.ValueObjects;

namespace Atoms.UnitTests.Core.Entities.Game;

public class CanPlayMoveTests
{
    [Test]
    public async Task ShouldNotBeAbleToPlayMoveWhenGameHasWinner()
    {
        var game = ObjectMother.Game(cells: [new(1, 1, 1, 1)], move: 3, round: 2);

        await Assert.That(
            game.CanPlayMove(ObjectMother.UserId, ObjectMother.VisitorId))
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
            game.CanPlayMove(ObjectMother.UserId, ObjectMother.VisitorId))
            .IsFalse();
    }

    [Test]
    public async Task ShouldBeAbleToPlayMoveWhenUserIdMatchesThatOfTheActivePlayer()
    {
        var player1 = ObjectMother.CreateHumanPlayer(
            ObjectMother.Player1Id, 1, ObjectMother.UserId);
        var player2 = ObjectMother.CreateCPUPlayer(ObjectMother.Player2Id, 2);

        var game = ObjectMother.Game(
            [player1, player2],
            userId: UserId.FromGuid(Guid.NewGuid()),
            visitorId: new VisitorId(Guid.NewGuid()));

        await Assert.That(
            game.CanPlayMove(ObjectMother.UserId, ObjectMother.VisitorId))
            .IsTrue();
    }

    [Test]
    public async Task ShouldBeAbleToPlayMoveWhenVisitorIdMatchesThatOfTheActivePlayer()
    {
        var player1 = ObjectMother.CreateHumanPlayer(
            ObjectMother.Player1Id, 1,
            visitorId: ObjectMother.VisitorId);
        var player2 = ObjectMother.CreateCPUPlayer(ObjectMother.Player2Id, 2);

        var game = ObjectMother.Game(
            [player1, player2],
            userId: UserId.FromGuid(Guid.NewGuid()),
            visitorId: new VisitorId(Guid.NewGuid()));

        await Assert.That(
            game.CanPlayMove(ObjectMother.UserId, ObjectMother.VisitorId))
            .IsTrue();
    }

    [Test]
    public async Task ShouldBeAbleToPlayMoveWhenActivePlayerDoesNotHaveAUserIdOrVisitorIdAndUserIdMatchesThatOfTheGame()
    {
        var player1 = ObjectMother.CreateHumanPlayer(ObjectMother.Player1Id, 1);
        var player2 = ObjectMother.CreateCPUPlayer(ObjectMother.Player2Id, 2);

        var game = ObjectMother.Game(
            [player1, player2],
            visitorId: new VisitorId(Guid.NewGuid()));

        await Assert.That(
            game.CanPlayMove(ObjectMother.UserId, ObjectMother.VisitorId))
            .IsTrue();
    }

    [Test]
    public async Task ShouldBeAbleToPlayMoveWhenActivePlayerDoesNotHaveAUserIdOrVisitorIdAndVisitorIdMatchesThatOfTheGame()
    {
        var player1 = ObjectMother.CreateHumanPlayer(ObjectMother.Player1Id, 1);
        var player2 = ObjectMother.CreateCPUPlayer(ObjectMother.Player2Id, 2);

        var game = ObjectMother.Game(
            [player1, player2],
            visitorId: new VisitorId(Guid.NewGuid()));

        await Assert.That(
            game.CanPlayMove(ObjectMother.UserId, ObjectMother.VisitorId))
            .IsTrue();
    }

    [Test]
    public async Task ShouldNotBeAbleToPlayMoveWhenActivePlayerDoesNotHaveAUserIdOrVisitorIdAndNeitherUserIdOrVisitorIdMatchesThatOfTheGame()
    {
        var player1 = ObjectMother.CreateHumanPlayer(ObjectMother.Player1Id, 1);
        var player2 = ObjectMother.CreateCPUPlayer(ObjectMother.Player2Id, 2);

        var game = ObjectMother.Game(
            [player1, player2],
            userId: UserId.FromGuid(Guid.NewGuid()),
            visitorId: new VisitorId(Guid.NewGuid()));

        await Assert.That(
            game.CanPlayMove(ObjectMother.UserId, ObjectMother.VisitorId))
            .IsFalse();
    }

    [Test]
    public async Task ShouldNotBeAbleToPlayMoveWhenUserIsLoggedInAndActivePlayerIsAUserWhoIsNotTheSameUser()
    {
        var player1 = ObjectMother.CreateHumanPlayer(
            ObjectMother.Player1Id, 1, ObjectMother.UserId.Id);
        var player2 = ObjectMother.CreateHumanPlayer(
            ObjectMother.Player2Id, 2, Guid.NewGuid().ToString());

        var game = ObjectMother.Game(
            [player1, player2], 2,
            userId: ObjectMother.UserId);

        await Assert.That(
            game.CanPlayMove(ObjectMother.UserId, ObjectMother.VisitorId))
            .IsFalse();
    }

    [Test]
    public async Task ShouldNotBeAbleToPlayMoveWhenUserHasAVisitorIdAndTheActivePlayerHasAVisitorIdThatDoesNotMatch()
    {
        var player1 = ObjectMother.CreateHumanPlayer(
            ObjectMother.Player1Id, 1,
            visitorId: ObjectMother.VisitorId);
        var player2 = ObjectMother.CreateHumanPlayer(
            ObjectMother.Player2Id, 2, 
            visitorId: new VisitorId(Guid.NewGuid()));

        var game = ObjectMother.Game(
            [player1, player2], 2);

        await Assert.That(
            game.CanPlayMove(null, ObjectMother.VisitorId))
            .IsFalse();
    }
}
