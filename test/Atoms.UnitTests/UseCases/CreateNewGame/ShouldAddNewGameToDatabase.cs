using Atoms.UseCases.CreateNewGame;

namespace Atoms.UnitTests.UseCases.CreateNewGame;

public class ShouldAddNewGameToDatabase : BaseDbTestFixture
{
    [Test]
    public async Task Test()
    {
        var game = ObjectMother.Game();
        var handler = new CreateNewGameRequestHandler(
            (options, userIdentity) => game, 
            DbContextFactory);

        var request = new CreateNewGameRequest(
            ObjectMother.GameMenuOptions, ObjectMother.UserIdentity);
        var response = await handler.Handle(request, CancellationToken.None);

        using var dbContext = await DbContextFactory.CreateDbContextAsync();

        var gameDto = await dbContext.Games.FindAsync(game.Id);

        await Assert.That(gameDto).IsNotNull()
             .And.HasMember(x => x!.Id).EqualTo(game.Id);
    }
}
