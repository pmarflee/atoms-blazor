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
        dbContextFactoryExpectations.Methods
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
}
