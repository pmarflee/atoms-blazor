using Atoms.UseCases.Invites.ReadInviteCode;
using static Atoms.UnitTests.ObjectMother;

namespace Atoms.UnitTests.UseCases.Invites;

public class ReadInviteCodeTests
{
    [Test]
    public async Task ShouldReturnFailureResponseWhenInviteCodeCannotBeRead()
    {
        var handler = new ReadInviteCodeRequestHandler();

        var result = await handler.Handle(
            new ReadInviteCodeRequest("ABC"),
            CancellationToken.None);

        await Assert.That(result.IsSuccessful).IsFalse();
    }

    [Test]
    public async Task ShouldReturnSuccessResponseWhenInviteIsValid()
    {
        var handler = new ReadInviteCodeRequestHandler();

        var result = await handler.Handle(
            new ReadInviteCodeRequest(Player1Id.ToString("N")),
            CancellationToken.None);

        await Assert.That(result.IsSuccessful).IsTrue();
    }
}
