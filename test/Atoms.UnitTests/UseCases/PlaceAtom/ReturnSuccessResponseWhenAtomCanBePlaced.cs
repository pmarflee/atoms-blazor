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
        var state = new Game.State
        {
            Rows = 10,
            Columns = 6,
            Players = 
            [ 
                new Game.State.Player 
                { 
                    Number = 1, 
                    Type = PlayerType.Human,
                    IsActive = true
                },
                new Game.State.Player
                {
                    Number = 2,
                    Type = PlayerType.Human
                }
            ],
            Cells = [] 
        };

        var game = new Game(state);

        var response = await _handler.Handle(
            new PlaceAtomRequest(game, game.Board[1, 1]), 
            CancellationToken.None);

        await Assert.That(response.IsSuccessful).IsTrue();
    }
}
