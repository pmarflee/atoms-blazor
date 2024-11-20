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
        var state = ObjectMother.NewGameState with
        {
            Players = [ new(1, PlayerType.Human), new(2, PlayerType.Human) ],
            ActivePlayer = 2,
            Cells = [ new(1, 1, 1, 1) ]
        };

        var game = Game.Load(state);

        var response = await _handler.Handle(
            new PlaceAtomRequest(game, game.Board[1, 1]), 
            CancellationToken.None);

        await Assert.That(response.IsSuccessful).IsFalse();
    }
}
