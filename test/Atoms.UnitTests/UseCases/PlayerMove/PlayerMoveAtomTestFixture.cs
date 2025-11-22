using Atoms.UseCases.PlayerMove;

namespace Atoms.UnitTests.UseCases.PlayerMove;

public abstract class PlayerMoveAtomTestFixture : BaseDbTestFixture
{
    internal IBusCreateExpectations BusExpectations = new();
    internal ILoggerCreateExpectations<PlayerMoveRequestHandler> LoggerExpectations = new();

    protected override async Task SetupInternal()
    {
        using var dbContext = await DbContextFactory.CreateDbContextAsync();

        await dbContext.LocalStorageUsers.AddAsync(new() { Id = GameState.LocalStorageUserId });
        await dbContext.Games.AddAsync(GameState);
        await dbContext.SaveChangesAsync();
    }

    protected virtual GameDTO GameState => ObjectMother.GameDTO();
}
