using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SmartEnum.EFCore;

namespace Atoms.Core.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options)
{
    public DbSet<GameDTO> Games { get; set; }
    public DbSet<PlayerDTO> Players { get; set; }
    public DbSet<PlayerTypeDTO> PlayerTypes { get; set; }
    public DbSet<LocalStorageUserDTO> LocalStorageUsers { get; set; }

    public IQueryable<GameInfoDTO> GetGamesForUser(
        StorageId localStorageId,
        UserId? userId)
    {
        var localStorageIdParam = new SqliteParameter(
            "localStorageId", localStorageId.Value);
        var userIdParam = new SqliteParameter(
            "userId", userId?.Id ?? (object)DBNull.Value);

        return Set<GameInfoDTO>().FromSqlRaw(
            """
            SELECT  g.Id, 
                    g.CreatedDateUtc, 
                    g.LastUpdatedDateUtc, 
                    g.Move,
                    g.Round,
                    g.IsActive,
                    (
                        SELECT 	    STRING_AGG(
                                        CASE pt.Name
                                        WHEN 'Human' THEN IFNULL(u.Name, 'Player ' || p.Number)
                                        ELSE pt.Description
                                        END, ', ') AS [Players]
                        FROM 		Players p
                        INNER JOIN  PlayerTypes pt 
                        ON          p.PlayerTypeId = pt.Id
                        LEFT JOIN   LocalStorageUsers u
                        ON          p.LocalStorageUserId = u.Id
                        WHERE		p.GameId = g.Id
                        AND         (p.LocalStorageUserId IS NULL OR p.LocalStorageUserId <> @localStorageId)
                        AND         (@userId IS NULL OR p.UserId <> @userId)
                        ORDER BY	p."Number"
                    ) AS [Opponents],
                    (
                        SELECT      CASE pt.Name
                                        WHEN 'Human' THEN IFNULL(u.Name, 'Player ' || p.Number)
                                        ELSE pt.Description
                                    END as [Player]
                        FROM        Players p
                        INNER JOIN  PlayerTypes pt
                        ON          p.PlayerTypeId = pt.Id
                        LEFT JOIN   LocalStorageUsers u
                        ON          p.LocalStorageUserId = u.Id
                        WHERE       p.GameId = g.Id
                        AND         p.IsWinner = 1
                    ) AS [Winner]
            FROM    Games g
            WHERE   
            (
                g.LocalStorageUserId = @localStorageId
                AND (@userId IS NULL OR g.UserId = @userId)
            )
            OR      EXISTS (
                        SELECT 1 
                        FROM Players p
                        WHERE p.GameId = g.Id 
                        AND (p.LocalStorageUserId = @localStorageId
                        AND (@userId IS NULL OR p.UserId = @userId))
                    )
            """, localStorageIdParam, userIdParam);
    }

    public async Task<GameDTO?> GetGameById(Guid id,
                                            CancellationToken cancellationToken)
    {
        return await Games.FindAsync([id], cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GameDTO>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<GameDTO>()
            .HasIndex(x => x.LocalStorageUserId);

        modelBuilder.Entity<GameDTO>()
            .HasIndex(x => x.UserId);

        modelBuilder.Entity<GameDTO>()
            .HasIndex(x => x.LastUpdatedDateUtc);

        modelBuilder.Entity<GameDTO>()
            .ComplexProperty(x => x.Board);

        modelBuilder.Entity<GameDTO>()
            .ComplexProperty(x => x.Rng);

        modelBuilder.Entity<GameDTO>()
            .HasOne(x => x.LocalStorageUser)
            .WithMany(x => x.Games)
            .HasForeignKey(x => x.LocalStorageUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<GameDTO>()
            .Navigation(x => x.LocalStorageUser)
            .AutoInclude();

        modelBuilder.Entity<GameDTO>()
            .Navigation(x => x.Players)
            .AutoInclude();

        modelBuilder.Entity<PlayerDTO>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<PlayerDTO>()
            .HasIndex(x => x.UserId);

        modelBuilder.Entity<PlayerDTO>()
            .HasIndex(x => x.LocalStorageUserId);

        modelBuilder.Entity<PlayerDTO>()
            .HasIndex(x => x.IsWinner);

        modelBuilder.Entity<PlayerDTO>()
            .HasOne(x => x.LocalStorageUser)
            .WithMany(x => x.Players)
            .HasForeignKey(x => x.LocalStorageUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PlayerDTO>()
            .Navigation(x => x.LocalStorageUser)
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
