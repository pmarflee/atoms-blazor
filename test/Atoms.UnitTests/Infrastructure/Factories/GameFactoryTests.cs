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
        var gameDto = CreateGame();

        using var _ = Assert.Multiple();

        await Assert.That(gameDto.Players.Count)
            .IsEqualTo(ObjectMother.GameMenuOptions.NumberOfPlayers);

        for (var i = 0; i < ObjectMother.GameMenuOptions.NumberOfPlayers; i++)
        {
            var player = gameDto.Players.ElementAt(i);

            await Assert.That(player.Number)
                .IsEqualTo(ObjectMother.GameMenuOptions.Players[i].Number);
            await Assert.That(player.PlayerTypeId)
                .IsEqualTo(ObjectMother.GameMenuOptions.Players[i].Type);
            await Assert.That(player.IsActive).IsEqualTo(i == 0);
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

    [Test]
    public async Task ShouldCreateGameWithCreationDateSetToCurrentUtcDateTime()
    {
        var utcNow = new DateTime(2026, 1, 11, 11, 31, 0);
        var game = CreateGame(utcNow);

        await Assert.That(game.CreatedDateUtc)
            .IsEqualTo(utcNow);
    }

    static GameDTO CreateGame(DateTime? utcNow = null)
    {
        var dateTimeServiceExpectations = new IDateTimeServiceCreateExpectations();
        dateTimeServiceExpectations.Setups
            .UtcNow
            .Gets().ReturnValue(utcNow ?? DateTime.UtcNow);

        return GameFactory.Create(
            ObjectMother.CreateRng,
            dateTimeServiceExpectations.Instance(),
            ObjectMother.GameMenuOptions,
            ObjectMother.LocalStorageId,
            gameId: ObjectMother.GameId);
    }
}
