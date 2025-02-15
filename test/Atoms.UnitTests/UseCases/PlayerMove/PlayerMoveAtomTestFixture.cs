using Atoms.UseCases.PlayerMove;
using Atoms.UseCases.Shared.Notifications;

namespace Atoms.UnitTests.UseCases.PlayerMove;

public abstract class PlayerMoveAtomTestFixture : BaseTestFixture
{
    protected IMediator Mediator { get; private set; } = default!;
    protected PlayerMoveRequestHandler Handler { get; private set; } = default!;

    protected override async Task SetupInternal()
    {
        using var dbContext = await DbContextFactory.CreateDbContextAsync();

        var gameDto = ObjectMother.GameDTO();

        await dbContext.Games.AddAsync(gameDto);

        await dbContext.SaveChangesAsync();

        Mediator = CreateMediator();
        Handler = new PlayerMoveRequestHandler(Mediator, DbContextFactory);
    }

    private static IMediator CreateMediator()
    {
        var mediatorExpectations = new IMediatorCreateExpectations();

        mediatorExpectations.Methods
            .Publish(Arg.Any<AtomPlaced>())
            .ReturnValue(Task.CompletedTask);
        mediatorExpectations.Methods
            .Publish(Arg.Any<AtomExploded>())
            .ReturnValue(Task.CompletedTask);

        return mediatorExpectations.Instance();
    }
}
