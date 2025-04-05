using Microsoft.EntityFrameworkCore;
using SmartEnum.EFCore;

namespace Atoms.Core.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options)
{
    public DbSet<GameDTO> Games { get; set; }
    public DbSet<PlayerDTO> Players { get; set; }

    public async Task<GameDTO?> GetGameById(Guid id,
                                            CancellationToken cancellationToken)
    {
        var gameDto = await Games.FindAsync([id], cancellationToken);

        if (gameDto is null) return null;

        await Entry(gameDto).Collection(x => x.Players)
                            .LoadAsync(cancellationToken);

        return gameDto;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GameDTO>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<GameDTO>()
            .HasIndex(x => x.LocalStorageId);

        modelBuilder.Entity<GameDTO>()
            .ComplexProperty(x => x.Board);

        modelBuilder.Entity<GameDTO>()
            .ComplexProperty(x => x.Rng);

        modelBuilder.Entity<PlayerDTO>()
            .HasKey(x => x.Id);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        configurationBuilder.ConfigureSmartEnum();
    }
}
