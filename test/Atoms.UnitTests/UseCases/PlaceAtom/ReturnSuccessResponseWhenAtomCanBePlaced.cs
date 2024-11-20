using Atoms.UseCases.PlaceAtom;

namespace Atoms.UnitTests.UseCases.PlaceAtom;

public class ReturnSuccessResponseWhenAtomCanBePlaced
{
    IMediator _mediator = default!;
    PlaceAtomRequestHandler _handler = default!;

    [Before(HookType.Test)]
    public Task Setup()
    {
        var mediatorExpectations = new IMediatorCreateExpectations();

        _mediator = mediatorExpectations.Instance();
        _handler = new PlaceAtomRequestHandler(_mediator);

        return Task.CompletedTask;
    }

    [Test]
    public async Task Test()
    {
        var game = Game.Load(ObjectMother.NewGameState);

        var response = await _handler.Handle(
            new PlaceAtomRequest(game, game.Board[1, 1]), 
            CancellationToken.None);

        await Assert.That(response.IsSuccessful).IsTrue();
    }
}
