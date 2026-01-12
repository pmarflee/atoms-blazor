using Atoms.UseCases.CreateNewGame;

namespace Atoms.UnitTests.UseCases.CreateNewGame;

public class ShouldReturnExpectedGameInstance : BaseDbTestFixture
{
    [Test]
    public async Task Test()
    {
        var gameDto = ObjectMother.GameDTO();

        var gameCreationServiceExpectations = new IGameCreationServiceCreateExpectations();
        gameCreationServiceExpectations.Setups
            .CreateGame(Arg.Any<GameMenuOptions>(),
                        Arg.Any<UserIdentity>(),
                        Arg.Any<CancellationToken>())
            .ReturnValue(Task.FromResult(gameDto));

        using var dbContext = await DbContextFactory.CreateDbContextAsync();

        await dbContext.LocalStorageUsers.AddAsync(ObjectMother.LocalStorageUser);
        await dbContext.SaveChangesAsync();

        var handler = new CreateNewGameRequestHandler(
            gameCreationServiceExpectations.Instance());

        var request = new CreateNewGameRequest(
            ObjectMother.GameMenuOptions,
            ObjectMother.UserIdentity);
        var response = await handler.Handle(request, CancellationToken.None);

        using var _ = Assert.Multiple();

        await Assert.That(response).IsNotNull();
        await Assert.That(response.Game).IsEqualTo(gameDto);
    }
}
