using Atoms.UseCases.CreateNewGame;

namespace Atoms.UnitTests.UseCases.CreateNewGame;

public class ShouldAddNewGameToDatabase : BaseDbTestFixture
{
    [Test]
    public async Task Test()
    {
        var gameDto = ObjectMother.GameDTO();

        using var dbContext = await DbContextFactory.CreateDbContextAsync();

        var gameCreationServiceExpectations = new IGameCreationServiceCreateExpectations();
        gameCreationServiceExpectations.Setups
            .CreateGame(Arg.Any<GameMenuOptions>(),
                        Arg.Any<UserIdentity>(),
                        Arg.Any<CancellationToken>())
            .Callback(async (options, identity, token) =>
            {
                await dbContext.Games.AddAsync(gameDto, CancellationToken.None);
                await dbContext.SaveChangesAsync(CancellationToken.None);

                return gameDto;
            });

        await dbContext.LocalStorageUsers.AddAsync(ObjectMother.LocalStorageUser);
        await dbContext.SaveChangesAsync();

        var handler = new CreateNewGameRequestHandler(
            gameCreationServiceExpectations.Instance());

        var request = new CreateNewGameRequest(
            ObjectMother.GameMenuOptions,
            ObjectMother.UserIdentity);
        var response = await handler.Handle(request, CancellationToken.None);

        var gameDtoFromDb = await dbContext.Games.FindAsync(gameDto.Id);

        await Assert.That(gameDtoFromDb).IsNotNull()
             .And.Member(x => x!.Id, x => x.EqualTo(gameDto.Id));
    }
}
