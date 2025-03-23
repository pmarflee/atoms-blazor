namespace Atoms.UnitTests.Core.Entities.Game;

public class ConstructorShouldCheckForDeadPlayers
{
    [Test, MethodDataSource(nameof(GetTestData))]
    public async Task Test(Atoms.Core.Entities.Game game, List<int> expected)
    {
        var deadPlayers = game.Players
            .Where(player => player.IsDead)
            .Select(player => player.Number)
            .ToList();

        await Assert.That(deadPlayers).IsEquivalentTo(expected);
    }

    public static IEnumerable<Func<(Atoms.Core.Entities.Game, List<int>)>> GetTestData()
    {
        yield return () => (ObjectMother.Game(), []);
        yield return () => (
            ObjectMother.Game(
                cells: [new(1, 2, 1, 2), new(2, 1, 1, 1)],
                move: 4,
                round: 2),
            [2]);
        yield return () => (
            ObjectMother.Game(
                cells: [new(1, 2, 2, 2), new(2, 1, 2, 1)],
                move: 5,
                round: 3),
            [1]);
        yield return () => (
            ObjectMother.Game(
                cells: [new(1, 1, 1, 1)],
                move: 2,
                round: 1),
            []);
        yield return () => (
            ObjectMother.Game(
                cells: [new(1, 1, 1, 1), new(1, 2, 2, 1)],
                move: 3,
                round: 2),
            []);
        yield return () => (
            ObjectMother.Game(
                players:
                [
                    new(Guid.NewGuid(), 1, PlayerType.Human),
                    new(Guid.NewGuid(), 2, PlayerType.Human),
                    new(Guid.NewGuid(), 3, PlayerType.Human)
                ],
                cells: [new(1, 1, 1, 1), new(1, 2, 2, 1)],
                move: 3,
                round: 1),
            []);
    }
}
