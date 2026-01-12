using Atoms.Core.Services;

namespace Atoms.UnitTests.Core.Services;

public class GameCreationServiceTests : BaseDbTestFixture
{
    [Test]
    public async Task TestCreateGame()
    {
        using var dbContext = await DbContextFactory.CreateDbContextAsync();

        var localStorageUserServiceExpectations = new ILocalStorageUserServiceCreateExpectations();
        localStorageUserServiceExpectations.Setups
            .GetOrAddLocalStorageId(Arg.Any<CancellationToken?>())
            .ReturnValue(Task.FromResult(ObjectMother.LocalStorageId));

        await dbContext.LocalStorageUsers.AddAsync(ObjectMother.LocalStorageUser);
        await dbContext.SaveChangesAsync();

        var gameDto = ObjectMother.GameDTO();

        var gameCreationService = new GameCreationService(
            localStorageUserServiceExpectations.Instance(),
            (gameId, options, localStorageId, userIdentity) => gameDto,
            DbContextFactory);

        var responseGameDto = await gameCreationService.CreateGame(
            ObjectMother.GameMenuOptions,
            ObjectMother.UserIdentity,
            CancellationToken.None);

        await Assert.That(responseGameDto).IsEqualTo(gameDto);

        var gameDtoFromDb = await dbContext.Games.FindAsync(gameDto.Id);

        await Assert.That(gameDtoFromDb).IsNotNull()
             .And.Member(x => x!.Id, x => x.EqualTo(gameDto.Id));

    }
}
