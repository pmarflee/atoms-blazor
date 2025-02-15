using Atoms.Core.Data;
using Microsoft.EntityFrameworkCore;

namespace Atoms.UnitTests;

internal class TestApplicationDbContextFactory(string databaseName = "Atoms") 
    : IDbContextFactory<ApplicationDbContext>
{
    private readonly DbContextOptions<ApplicationDbContext> _options = 
        new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;

    public ApplicationDbContext CreateDbContext()
    {
        return new ApplicationDbContext(_options);
    }
}
