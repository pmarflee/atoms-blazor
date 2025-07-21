using Atoms.Infrastructure.Factories;

namespace Atoms.UnitTests.Infrastructure.Factories;

public class GameFactoryTests
{
    [Test]
    public async Task ShouldCreateGameWithSpecifiedId()
    {
        var game = CreateGame();

        await Assert.That(game.Id).IsEqualTo(ObjectMother.GameId);
    }

    [Test]
    public async Task ShouldCreateGameWithSpecifiedPlayers()
    {
        var game = CreateGame();

        using var _ = Assert.Multiple();

        await Assert.That(game.Players.Count)
            .IsEqualTo(ObjectMother.GameMenuOptions.NumberOfPlayers);

        for (var i = 0; i < ObjectMother.GameMenuOptions.NumberOfPlayers; i++)
        {
            await Assert.That(game.Players[i].Number)
                .IsEqualTo(ObjectMother.GameMenuOptions.Players[i].Number);
            await Assert.That(game.Players[i].Type)
                .IsEqualTo(ObjectMother.GameMenuOptions.Players[i].Type);
            await Assert.That(game.Players[i].IsDead).IsFalse();
        }
    }

    [Test]
    public async Task ShouldCreateGameWithSpecifiedColourScheme()
    {
        var game = CreateGame();

        await Assert.That(game.ColourScheme)
            .IsEqualTo(ObjectMother.GameMenuOptions.ColourScheme);
    }

    [Test]
    public async Task ShouldCreateGameWithSpecifiedAtomShape()
    {
        var game = CreateGame();

        await Assert.That(game.AtomShape)
            .IsEqualTo(ObjectMother.GameMenuOptions.AtomShape);
    }

    static Game CreateGame()
    {
        return GameFactory.Create(
            ObjectMother.CreateRng,
            ObjectMother.CreatePlayerStrategy,
            ObjectMother.InviteSerializer,
            ObjectMother.GameMenuOptions,
            ObjectMother.LocalStorageId);
    }
}
