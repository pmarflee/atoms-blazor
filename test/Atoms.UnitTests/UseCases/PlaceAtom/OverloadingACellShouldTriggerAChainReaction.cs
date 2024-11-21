using Atoms.UseCases.PlaceAtom;
using Atoms.UseCases.Shared.Notifications;

namespace Atoms.UnitTests.UseCases.PlaceAtom;

public class OverloadingACellShouldTriggerAChainReaction
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

    [Test, MethodDataSource(nameof(GetTestData))]
    public async Task Test(Game game,
                           int row,
                           int column,
                           Game expected)
    {
        var cell = game.Board[row, column];

        await _handler.Handle(
            new PlaceAtomRequest(game, cell), 
            CancellationToken.None);

        await Assert.That(game).IsEquivalentTo(expected);
    }

    public static IEnumerable<(Game, int, int, Game)> GetTestData()
    {
        yield return (
            ObjectMother.Game(cells: [new(1, 1, 1, 1)]),
            1, 1,
            ObjectMother.Game(
                active: 2,
                cells: [new(1, 2, 1, 1), new(2, 1, 1, 1)])
        );

        yield return (
            ObjectMother.Game(cells: [new(1, 2, 1, 2)]),
            1, 2,
            ObjectMother.Game(
                active: 2,
                cells: [ new(1, 1, 1, 1), new(1, 3, 1, 1),
                         new(2, 2, 1, 1) ])
        );

        yield return (
            ObjectMother.Game(cells: [new(2, 2, 1, 3)]),
            2, 2,
            ObjectMother.Game(
                active: 2,
                cells: [ new(1, 2, 1, 1), new(2, 1, 1, 1),
                         new(2, 3, 1, 1), new(3, 2, 1, 1) ])
        );
    }
}
