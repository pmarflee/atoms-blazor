using Atoms.UseCases.PlaceAtom;

namespace Atoms.UnitTests.UseCases.PlaceAtom;

public class ReturnFailureResponseWhenAtomCannotBePlaced
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
        var game = ObjectMother.Game(
            active: 2,
            cells: [ new(1, 1, 1, 1) ]);

        var response = await _handler.Handle(
            new PlaceAtomRequest(game, game.Board[1, 1]), 
            CancellationToken.None);

        await Assert.That(response.IsSuccessful).IsFalse();
    }
}
