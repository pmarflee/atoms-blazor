using Atoms.Core.DTOs;

namespace Atoms.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<GameDTO> Games { get; set; }
    public DbSet<PlayerDTO> Players { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GameDTO>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<GameDTO>()
            .HasIndex(x => x.LocalStorageId);

        modelBuilder.Entity<GameDTO>()
            .ComplexProperty(x => x.Board);

        modelBuilder.Entity<PlayerDTO>()
            .HasKey(x => x.Id);
    }
}
