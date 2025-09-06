using Atoms.UseCases.CreateNewGame;

namespace Atoms.UnitTests.UseCases.CreateNewGame;

public class ShouldReturnExpectedGameInstance : BaseDbTestFixture
{
    [Test]
    public async Task Test()
    {
        var localStorageUserServiceExpectations = new ILocalStorageUserServiceCreateExpectations();
        localStorageUserServiceExpectations.Methods
            .GetOrAddLocalStorageId(Arg.Any<CancellationToken>())
            .ReturnValue(Task.FromResult(ObjectMother.LocalStorageId));

        using var dbContext = await DbContextFactory.CreateDbContextAsync();

        await dbContext.LocalStorageUsers.AddAsync(ObjectMother.LocalStorageUser);
        await dbContext.SaveChangesAsync();

        var game = ObjectMother.Game();
        var handler = new CreateNewGameRequestHandler(
            localStorageUserServiceExpectations.Instance(),
            (gameId, options, localStorageId, userIdentity) => game, 
            DbContextFactory);

        var request = new CreateNewGameRequest(
            ObjectMother.GameId, ObjectMother.GameMenuOptions,
            ObjectMother.UserIdentity);
        var response = await handler.Handle(request, CancellationToken.None);

        using var _ = Assert.Multiple();

        await Assert.That(response).IsNotNull();
        await Assert.That(response.Game).IsEqualTo(game);
    }
}
