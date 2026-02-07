using Atoms.Core.DTOs;
using Atoms.UseCases.Menu.GameOptions;

namespace Atoms.UnitTests.UseCases;

public class CreateGameMenuOptionsTests
{
    [Test]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2012:Use ValueTasks correctly", Justification = "<Pending>")]
    public async Task OptionsCreatedShouldMatchSpecification()
    {
        var browserStorageServiceExpectations = new IBrowserStorageServiceCreateExpectations();
        browserStorageServiceExpectations.Setups
            .GetGameMenuOptions()
            .ReturnValue(
                ValueTask.FromResult<GameMenuOptions?>(
                    ObjectMother.CreateGameMenuOptions(
                        Atoms.Core.Constants.MaxPlayers)));

        var handler = new CreateGameOptionsRequestHandler(browserStorageServiceExpectations.Instance());
        var request = new CreateGameOptionsRequest(
            Atoms.Core.Constants.MaxPlayers,
            ObjectMother.VisitorId, ObjectMother.UserId);

        var response = await handler.Handle(request, CancellationToken.None);
        var options = response.Options;

        using var _ = Assert.Multiple();

        await Assert.That(options.Players.Count).IsEqualTo(request.NumberOfPlayers);

        for (var i = 0; i < request.NumberOfPlayers; i++)
        {
            await Assert.That(options.Players[i].Number).IsEqualTo(i + 1);
            await Assert.That(options.Players[i].Type).IsEqualTo(PlayerType.Human);
        }
    }
}
