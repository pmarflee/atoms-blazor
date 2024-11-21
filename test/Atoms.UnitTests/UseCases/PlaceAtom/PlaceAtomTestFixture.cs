using Atoms.UseCases.PlaceAtom;
using Atoms.UseCases.Shared.Notifications;

namespace Atoms.UnitTests.UseCases.PlaceAtom;

public abstract class PlaceAtomTestFixture
{
    protected IMediator Mediator { get; private set; } = default!;
    protected PlaceAtomRequestHandler Handler { get; private set; } = default!;

    [Before(Test)]
    public Task Setup()
    {
        var mediatorExpectations = new IMediatorCreateExpectations();
        mediatorExpectations.Methods
            .Publish(Arg.Any<GameStateChanged>())
            .ReturnValue(Task.CompletedTask);

        Mediator = mediatorExpectations.Instance();
        Handler = new PlaceAtomRequestHandler(Mediator);

        return Task.CompletedTask;
    }
}
