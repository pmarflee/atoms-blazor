using Atoms.UseCases.PlayerMove;
using Atoms.UseCases.Shared.Notifications;

namespace Atoms.UnitTests.UseCases.PlayerMove;

public abstract class PlayerMoveAtomTestFixture : BaseDbTestFixture
{
    protected IMediator Mediator { get; private set; } = default!;
    protected PlayerMoveRequestHandler Handler { get; private set; } = default!;

    protected override async Task SetupInternal()
    {
        using var dbContext = await DbContextFactory.CreateDbContextAsync();

        await dbContext.LocalStorageUsers.AddAsync(new() { Id = GameState.LocalStorageUserId });
        await dbContext.Games.AddAsync(GameState);
        await dbContext.SaveChangesAsync();

        Mediator = CreateMediator();
        Handler = new PlayerMoveRequestHandler(
            Mediator, DbContextFactory, CreateDateTimeService());
    }

    protected virtual GameDTO GameState => ObjectMother.GameDTO();

    private static IMediator CreateMediator()
    {
        var mediatorExpectations = new IMediatorCreateExpectations();

        mediatorExpectations.Methods
            .Publish(Arg.Any<AtomPlaced>())
            .ReturnValue(Task.CompletedTask);
        mediatorExpectations.Methods
            .Publish(Arg.Any<AtomExploded>())
            .ReturnValue(Task.CompletedTask);
        mediatorExpectations.Methods
            .Publish(Arg.Any<PlayerMoved>())
            .ReturnValue(Task.CompletedTask);
        mediatorExpectations.Methods
            .Publish(Arg.Any<GameSaved>())
            .ReturnValue(Task.CompletedTask);
        mediatorExpectations.Methods
            .Publish(Arg.Any<PlayerJoined>())
            .ReturnValue(Task.CompletedTask);

        return mediatorExpectations.Instance();
    }

    private static IDateTimeService CreateDateTimeService()
    {
        var dateTimeServiceExpectations = new IDateTimeServiceCreateExpectations();

        dateTimeServiceExpectations.Properties.Getters.UtcNow()
            .ReturnValue(ObjectMother.NewLastUpdatedDateUtc);

        return dateTimeServiceExpectations.Instance();
            
    }
}
