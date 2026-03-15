using Atoms.Core.Data.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Atoms.UnitTests;

public abstract class BaseDbTestFixture
{
    private SqliteConnection _connection = default!;

    protected IDbContextFactory<ApplicationDbContext> DbContextFactory { get; private set; } = default!;

    [Before(Test)]
    public async Task Setup()
    {
        await SetupApplicationDbContext();
        await SetupInternal();
    }

    [After(Test)]
    public void Teardown()
    {
        _connection.Close();
        _connection.Dispose();
    }

    protected virtual Task SetupInternal() => Task.CompletedTask;

    async Task SetupApplicationDbContext()
    {
        _connection = new("Filename=:memory:");
        _connection.Open();

        var dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(_connection)
            .Options;

        var dbContextFactoryExpectations = new IDbContextFactoryCreateExpectations<ApplicationDbContext>();
        dbContextFactoryExpectations.Setups
            .CreateDbContextAsync(Arg.Any<CancellationToken>())
            .Callback(token => Task.FromResult(
                new ApplicationDbContext(dbContextOptions)));

        DbContextFactory = dbContextFactoryExpectations.Instance();

        using var dbContext = await DbContextFactory.CreateDbContextAsync();

        var isCreated = await dbContext.Database.EnsureCreatedAsync();

        if (!isCreated)
        {
            throw new Exception("Unable to create test database");
        }
    }

    protected static IDbContextFactory<ApplicationIdentityDbContext> CreateIdentityDbContextFactory()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();

        var dbContextOptions = new DbContextOptionsBuilder<ApplicationIdentityDbContext>()
            .UseSqlite(connection)
            .Options;

        var dbContextFactoryExpectations = new IDbContextFactoryCreateExpectations<ApplicationIdentityDbContext>();
        dbContextFactoryExpectations.Setups
            .CreateDbContextAsync(Arg.Any<CancellationToken>())
            .Callback(async token =>
            {
                var context = new ApplicationIdentityDbContext(dbContextOptions);
                await context.Database.EnsureCreatedAsync(token);
                return context;
            });

        return dbContextFactoryExpectations.Instance();
    }
}
