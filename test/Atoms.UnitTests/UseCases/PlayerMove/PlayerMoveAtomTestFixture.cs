using Atoms.UseCases.PlayerMove;
using Atoms.UseCases.Shared.Notifications;

namespace Atoms.UnitTests.UseCases.PlayerMove;

public abstract class PlayerMoveAtomTestFixture
{
    protected IMediator Mediator { get; private set; } = default!;
    protected PlayerMoveRequestHandler Handler { get; private set; } = default!;

    [Before(Test)]
    public Task Setup()
    {
        var mediatorExpectations = new IMediatorCreateExpectations();
        mediatorExpectations.Methods
            .Publish(Arg.Any<AtomPlaced>())
            .ReturnValue(Task.CompletedTask);
        mediatorExpectations.Methods
            .Publish(Arg.Any<AtomExploded>())
            .ReturnValue(Task.CompletedTask);

        Mediator = mediatorExpectations.Instance();
        Handler = new PlayerMoveRequestHandler(Mediator);

        return Task.CompletedTask;
    }
}
