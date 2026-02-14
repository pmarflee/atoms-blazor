using Npgsql;
using Microsoft.EntityFrameworkCore;
using SmartEnum.EFCore;

namespace Atoms.Core.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options)
{
    public DbSet<GameDTO> Games { get; set; }
    public DbSet<PlayerDTO> Players { get; set; }
    public DbSet<PlayerTypeDTO> PlayerTypes { get; set; }
    public DbSet<VisitorDTO> Visitors { get; set; }

    public IQueryable<GameInfoDTO> GetGamesForUser(
        VisitorId visitorId,
        UserId? userId)
    {
        return Set<GameInfoDTO>().FromSql(
            $"SELECT * FROM get_games_for_user({visitorId.Value}, {userId?.Id})");
    }

    public async Task<GameDTO?> GetGameById(
        Guid id, CancellationToken cancellationToken)
    {
        return await Games.FindAsync([id], cancellationToken);
    }

    public async Task<GameDTO?> GetGameByPlayerId(
        Guid playerId, CancellationToken cancellationToken)
    {
        return await Games.FirstOrDefaultAsync(
            game => game.Players.Any(player => player.Id == playerId),
            cancellationToken: cancellationToken);
    }

    public async Task AddOrUpdateVisitor(
        VisitorId visitorId, string? name, CancellationToken cancellationToken)
    {
        var visitor = await Visitors.FindAsync(
            [visitorId.Value], cancellationToken);

        if (visitor is null)
        {
            visitor = new VisitorDTO { Id = visitorId.Value };

            Visitors.Add(visitor);
        }

        if (!string.IsNullOrEmpty(name))
        {
            visitor.Name = name;
        }

        await SaveChangesAsync(cancellationToken);
    }

    public ValueTask<VisitorDTO> GetVisitorById(VisitorId visitorId) =>
        FindAsync<VisitorDTO>(visitorId.Value)!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GameDTO>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<GameDTO>()
            .HasIndex(x => x.VisitorId);

        modelBuilder.Entity<GameDTO>()
            .HasIndex(x => x.UserId);

        modelBuilder.Entity<GameDTO>()
            .HasIndex(x => x.LastUpdatedDateUtc);

        modelBuilder.Entity<GameDTO>()
            .ComplexProperty(x => x.Board);

        modelBuilder.Entity<GameDTO>()
            .ComplexProperty(x => x.Rng);

        modelBuilder.Entity<GameDTO>()
            .HasOne(x => x.Visitor)
            .WithMany(x => x.Games)
            .HasForeignKey(x => x.VisitorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<GameDTO>()
            .Navigation(x => x.Visitor)
            .AutoInclude();

        modelBuilder.Entity<GameDTO>()
            .Navigation(x => x.Players)
            .AutoInclude();

        modelBuilder.Entity<PlayerDTO>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<PlayerDTO>()
            .HasIndex(x => x.UserId);

        modelBuilder.Entity<PlayerDTO>()
            .HasIndex(x => x.VisitorId);

        modelBuilder.Entity<PlayerDTO>()
            .HasIndex(x => x.IsWinner);

        modelBuilder.Entity<PlayerDTO>()
            .HasOne(x => x.Visitor)
            .WithMany(x => x.Players)
            .HasForeignKey(x => x.VisitorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PlayerDTO>()
            .Navigation(x => x.Visitor)
            .AutoInclude();

        modelBuilder.Entity<GameInfoDTO>()
            .HasNoKey()
            .ToView(null);

        modelBuilder.Entity<PlayerTypeDTO>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<PlayerTypeDTO>()
            .HasData(
                new PlayerTypeDTO
                {
                    Id = PlayerType.Human,
                    Name = PlayerType.Human.Name,
                    Description = PlayerType.Human.Description
                },
                new PlayerTypeDTO
                {
                    Id = PlayerType.CPU_Easy,
                    Name = PlayerType.CPU_Easy.Name,
                    Description = PlayerType.CPU_Easy.Description
                },
                new PlayerTypeDTO
                {
                    Id = PlayerType.CPU_Medium,
                    Name = PlayerType.CPU_Medium.Name,
                    Description = PlayerType.CPU_Medium.Description
                },
                new PlayerTypeDTO
                {
                    Id = PlayerType.CPU_Hard,
                    Name = PlayerType.CPU_Hard.Name,
                    Description = PlayerType.CPU_Hard.Description
                }
            );
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        configurationBuilder.ConfigureSmartEnum();
    }
}
