using Atoms.UseCases.Menu.GameOptions;

namespace Atoms.UnitTests.UseCases;

public class CreateGameMenuOptionsTests
{
    [Test]
    public async Task OptionsCreatedShouldMatchSpecification()
    {
        var browserStorageServiceExpectations = new IBrowserStorageServiceCreateExpectations();
        browserStorageServiceExpectations.Methods
            .GetColourScheme()
            .ReturnValue(ValueTask.FromResult(ColourScheme.Original));
        browserStorageServiceExpectations.Methods
            .GetAtomShape()
            .ReturnValue(ValueTask.FromResult(AtomShape.Round));

        var handler = new CreateGameOptionsRequestHandler(browserStorageServiceExpectations.Instance());
        var request = new CreateGameOptionsRequest(
            ObjectMother.GameId, 4,
            ObjectMother.LocalStorageId, ObjectMother.UserId);

        var response = await handler.Handle(request, CancellationToken.None);
        var options = response.Options;

        using var _ = Assert.Multiple();

        await Assert.That(options.GameId).IsEqualTo(request.GameId);
        await Assert.That(options.Players.Count).IsEqualTo(request.NumberOfPlayers);

        for (var i = 0; i < request.NumberOfPlayers; i++)
        {
            await Assert.That(options.Players[i].Number).IsEqualTo(i + 1);
            await Assert.That(options.Players[i].Type).IsEqualTo(PlayerType.Human);
        }
    }
}
