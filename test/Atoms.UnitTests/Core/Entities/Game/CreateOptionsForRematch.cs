using GameEntity = Atoms.Core.Entities.Game;

namespace Atoms.UnitTests.Core.Entities.Game;

public class CreateOptionsForRematch
{
    [Test]
    public void ShouldThrowExceptionWhenGameIsNotFinished()
    {
        var game = ObjectMother.Game();

        Assert.Throws<InvalidOperationException>(
            () => game.CreateOptionsForRematch(false));
    }

    [Test, MethodDataSource(nameof(GetTestData))]
    public async Task TestCreateOptions(
        GameEntity game, bool hasSound, GameMenuOptions expected)
    {
        var options = game.CreateOptionsForRematch(hasSound);

        await Assert.That(options).IsEquivalentTo(expected);
    }

    [Test]
    public async Task FirstPlayerShouldBeHumanIfThereWasAtLeastOneLosingHumanInThePreviousGame()
    {
        var game = ObjectMother.Game(
            players: [
                ObjectMother.CreateHumanPlayer(Guid.NewGuid(), 1),
                ObjectMother.CreateCPUPlayer(Guid.NewGuid(), 2, PlayerType.CPU_Easy),
                ObjectMother.CreateHumanPlayer(Guid.NewGuid(), 3)
            ],
            cells: [new CellState(2, 1, 1, 2), new CellState(1, 2, 1, 1)],
            move: 4, round: 2);
        game.Players[1].MarkDead();
        game.Players[2].MarkDead();

        var options = game.CreateOptionsForRematch(false);

        await Assert.That(options.Players[0].Type).IsEqualTo(PlayerType.Human);
    }

    [Test]
    public async Task FirstPlayerShouldNotBeHumanIfThereWereNoLosingHumansInThePreviousGame()
    {
        var game = ObjectMother.Game(
            players: [
                ObjectMother.CreateHumanPlayer(Guid.NewGuid(), 1),
                ObjectMother.CreateCPUPlayer(Guid.NewGuid(), 2, PlayerType.CPU_Easy),
                ObjectMother.CreateCPUPlayer(Guid.NewGuid(), 3, PlayerType.CPU_Medium),
                ObjectMother.CreateCPUPlayer(Guid.NewGuid(), 4, PlayerType.CPU_Hard)
            ],
            cells: [new CellState(2, 1, 1, 2), new CellState(1, 2, 1, 1)],
            move: 4, round: 2);
        game.Players[1].MarkDead();
        game.Players[2].MarkDead();
        game.Players[3].MarkDead();

        var options = game.CreateOptionsForRematch(false);

        await Assert.That(options.Players[0].Type).IsNotEqualTo(PlayerType.Human);
    }

    public static IEnumerable<Func<(GameEntity, bool, GameMenuOptions)>> GetTestData()
    {
        var game1 = ObjectMother.Game(
            cells: [new CellState(2, 1, 1, 2), new CellState(1, 2, 1, 1)],
            move: 4, round: 2);
        game1.Players[1].MarkDead();

        yield return () => 
        (
            game1,
            false,
            new GameMenuOptions
            {
                AtomShape = game1.AtomShape,
                ColourScheme = game1.ColourScheme,
                HasSound = false,
                NumberOfPlayers = game1.Players.Count,
                Players =
                [
                    new GameMenuOptions.Player
                    {
                        Number = 1,
                        Type = PlayerType.Human,
                        UserIdentity = new Atoms.Core.Entities.UserIdentity()
                    },
                    new GameMenuOptions.Player
                    {
                        Number = 2,
                        Type = PlayerType.Human,
                        UserIdentity = new Atoms.Core.Entities.UserIdentity(),
                        LocalStorageId = game1.Players[0].LocalStorageId
                    }
                ],
                IsRematch = true
            }
        );

        var game2 = ObjectMother.Game(
            players: [
                ObjectMother.CreateHumanPlayer(
                    Guid.NewGuid(),
                    1,
                    new(Guid.NewGuid().ToString()),
                    ObjectMother.LocalStorageId,
                    "Paul"),
                ObjectMother.CreateCPUPlayer(
                    Guid.NewGuid(),
                    2,
                    PlayerType.CPU_Easy)
            ],
            cells: [new CellState(2, 1, 1, 2), new CellState(1, 2, 1, 1)],
            move: 4, round: 2);
        game2.Players[1].MarkDead();

        yield return () => 
        (
            game2,
            false,
            new GameMenuOptions
            {
                AtomShape = game2.AtomShape,
                ColourScheme = game2.ColourScheme,
                HasSound = false,
                NumberOfPlayers = game2.Players.Count,
                Players =
                [
                    new GameMenuOptions.Player
                    {
                        Number = 1,
                        Type = PlayerType.CPU_Easy
                    },
                    new GameMenuOptions.Player
                    {
                        Number = 2,
                        Type = PlayerType.Human,
                        UserIdentity = new Atoms.Core.Entities.UserIdentity(
                            game2.Players[0].UserId,
                            game2.Players[0].Name),
                        LocalStorageId = game2.Players[0].LocalStorageId
                    }
                ],
                IsRematch = true
            }
        );
    }
}
