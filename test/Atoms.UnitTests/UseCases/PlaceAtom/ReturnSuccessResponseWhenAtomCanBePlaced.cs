using Atoms.UseCases.PlaceAtom;
using Atoms.UseCases.Shared.Notifications;

namespace Atoms.UnitTests.UseCases.PlaceAtom;

public class ReturnSuccessResponseWhenAtomCanBePlaced
{
    IMediator _mediator = default!;
    PlaceAtomRequestHandler _handler = default!;

    [Before(HookType.Test)]
    public Task Setup()
    {
        var mediatorExpectations = new IMediatorCreateExpectations();
        mediatorExpectations.Methods
            .Publish(Arg.Any<GameStateChanged>())
            .ReturnValue(Task.CompletedTask);

        _mediator = mediatorExpectations.Instance();
        _handler = new PlaceAtomRequestHandler(_mediator);

        return Task.CompletedTask;
    }

    [Test]
    public async Task Test()
    {
        var game = ObjectMother.Game();

        var response = await _handler.Handle(
            new PlaceAtomRequest(game, game.Board[1, 1]), 
            CancellationToken.None);

        await Assert.That(response.IsSuccessful).IsTrue();
    }
}
