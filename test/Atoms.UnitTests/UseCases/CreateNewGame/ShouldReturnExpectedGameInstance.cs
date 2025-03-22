using Atoms.UseCases.CreateNewGame;

namespace Atoms.UnitTests.UseCases.CreateNewGame;

public class ShouldReturnExpectedGameInstance : BaseDbTestFixture
{
    [Test]
    public async Task Test()
    {
        var game = ObjectMother.Game();
        var handler = new CreateNewGameRequestHandler(
            options => game, 
            DbContextFactory);

        var request = new CreateNewGameRequest(ObjectMother.GameMenuOptions);
        var response = await handler.Handle(request, CancellationToken.None);

        using var _ = Assert.Multiple();

        await Assert.That(response).IsNotNull();
        await Assert.That(response.Game).IsEqualTo(game);
    }
}
