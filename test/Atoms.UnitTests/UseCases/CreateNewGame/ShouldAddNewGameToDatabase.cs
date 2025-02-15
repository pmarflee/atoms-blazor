using Atoms.UseCases.CreateNewGame;

namespace Atoms.UnitTests.UseCases.CreateNewGame;

public class ShouldAddNewGameToDatabase : BaseTestFixture
{
    [Test]
    public async Task Test()
    {
        var game = ObjectMother.Game();
        var handler = new CreateNewGameRequestHandler(
            options => game, 
            DbContextFactory);

        var request = new CreateNewGameRequest(
            ObjectMother.GameMenuOptions,
            ObjectMother.LocalStorageId);

        var response = await handler.Handle(request, CancellationToken.None);

        using var dbContext = await DbContextFactory.CreateDbContextAsync();

        var gameDto = await dbContext.Games.FindAsync(game.Id);

        await Assert.That(gameDto).IsNotNull()
             .And.HasMember(x => x!.Id).EqualTo(game.Id);
    }
}
