using Atoms.Core.Data.Identity;
using Atoms.Core.Identity;
using Atoms.UseCases.DeleteUserData;

namespace Atoms.UnitTests.UseCases.DeleteUserData;

public class DeleteUserDataRequestHandlerTests : BaseDbTestFixture
{
    private IDbContextFactory<ApplicationIdentityDbContext> _identityDbContextFactory = default!;

    [Test]
    public async Task ShouldDeleteOwnedGamesEntirely()
    {
        var userId = ObjectMother.UserId;
        var gameId = Guid.NewGuid();

        using (var dbContext = await DbContextFactory.CreateDbContextAsync())
        {
            var gameDto = CreateGameDtoOwnedByUser(gameId, userId);

            await dbContext.Visitors.AddAsync(ObjectMother.VisitorUser);
            await dbContext.Games.AddAsync(gameDto);
            await dbContext.SaveChangesAsync();
        }

        var handler = new DeleteUserDataRequestHandler(DbContextFactory, _identityDbContextFactory);
        await handler.Handle(new DeleteUserDataRequest(ObjectMother.VisitorId, userId), CancellationToken.None);

        using (var dbContext = await DbContextFactory.CreateDbContextAsync())
        {
            var deletedGame = await dbContext.Games.FindAsync(gameId);
            await Assert.That(deletedGame).IsNull();
        }
    }

    [Test]
    public async Task ShouldSetUserIdToNullForPlayerEntriesInNonOwnedGames()
    {
        var userId = ObjectMother.UserId;
        var otherVisitorId = new VisitorId(Guid.NewGuid());
        var gameId = Guid.NewGuid();
        var player1Id = Guid.NewGuid();

        using (var dbContext = await DbContextFactory.CreateDbContextAsync())
        {
            var otherVisitor = new VisitorDTO { Id = otherVisitorId.Value, Name = "Other Visitor" };
            var yetAnotherVisitor = new VisitorDTO { Id = Guid.NewGuid(), Name = "Yet Another Visitor" };
            await dbContext.Visitors.AddAsync(ObjectMother.VisitorUser);
            await dbContext.Visitors.AddAsync(otherVisitor);
            await dbContext.Visitors.AddAsync(yetAnotherVisitor);

            var gameDto = new GameDTO
            {
                Id = gameId,
                UserId = null, // Owned by visitor, not user
                VisitorId = otherVisitorId.Value,
                ColourScheme = ColourScheme.Original,
                AtomShape = AtomShape.Round,
                Board = ObjectMother.BoardDTO(),
                Move = 1,
                Round = 1,
                IsActive = true,
                Rng = new RngDTO { Seed = 1, Iterations = 0 },
                CreatedDateUtc = ObjectMother.CreatedDateUtc,
                LastUpdatedDateUtc = ObjectMother.LastUpdatedDateUtc
            };

            var player = CreatePlayerDto(player1Id, 1, userId);
            player.Game = gameDto;
            gameDto.Players.Add(player);

            var player2 = CreateCpuPlayerDto(Guid.NewGuid(), 2);
            player2.Game = gameDto;
            gameDto.Players.Add(player2);

            await dbContext.Games.AddAsync(gameDto);
            await dbContext.SaveChangesAsync();
        }

        var handler = new DeleteUserDataRequestHandler(DbContextFactory, _identityDbContextFactory);
        await handler.Handle(new DeleteUserDataRequest(ObjectMother.VisitorId, userId), CancellationToken.None);

        using (var dbContext = await DbContextFactory.CreateDbContextAsync())
        {
            var game = await dbContext.Games.FindAsync(gameId);

            await Assert.That(game).IsNotNull();
            var playerFromDb = game!.Players.Single(p => p.Id == player1Id);
            await Assert.That(playerFromDb.UserId).IsNull();
        }
    }

    [Test]
    public async Task ShouldHandleMultiplePlayersLinkedToSameUserInNonOwnedGames()
    {
        var userId = ObjectMother.UserId;
        var game1Id = Guid.NewGuid();
        var game2Id = Guid.NewGuid();

        using (var dbContext = await DbContextFactory.CreateDbContextAsync())
        {
            var otherVisitorId1 = Guid.NewGuid();
            var otherVisitorId2 = Guid.NewGuid();
            await dbContext.Visitors.AddAsync(ObjectMother.VisitorUser);
            await dbContext.Visitors.AddAsync(new VisitorDTO { Id = otherVisitorId1, Name = "Visitor 1" });
            await dbContext.Visitors.AddAsync(new VisitorDTO { Id = otherVisitorId2, Name = "Visitor 2" });

            var game1 = CreateGameDtoOwnedByVisitor(game1Id, otherVisitorId1);
            var player1 = CreatePlayerDto(Guid.NewGuid(), 1, userId);
            player1.Game = game1;
            game1.Players.Add(player1);
            var player2 = CreateCpuPlayerDto(Guid.NewGuid(), 2);
            player2.Game = game1;
            game1.Players.Add(player2);

            var game2 = CreateGameDtoOwnedByVisitor(game2Id, otherVisitorId2);
            var player3 = CreatePlayerDto(Guid.NewGuid(), 1, userId);
            player3.Game = game2;
            game2.Players.Add(player3);
            var player4 = CreateCpuPlayerDto(Guid.NewGuid(), 2);
            player4.Game = game2;
            game2.Players.Add(player4);

            await dbContext.Games.AddRangeAsync(game1, game2);
            await dbContext.SaveChangesAsync();
        }

        var handler = new DeleteUserDataRequestHandler(DbContextFactory, _identityDbContextFactory);
        await handler.Handle(new DeleteUserDataRequest(ObjectMother.VisitorId, userId), CancellationToken.None);

        using (var dbContext = await DbContextFactory.CreateDbContextAsync())
        {
            var players = await dbContext.Players.Where(p => p.UserId == null && p.PlayerTypeId == PlayerType.Human).ToListAsync();
            await Assert.That(players.Count).EqualTo(2);
        }
    }

    [Test]
    public async Task ShouldNotAffectOtherPlayersInNonOwnedGames()
    {
        var userId = ObjectMother.UserId;
        var otherVisitorId = new VisitorId(Guid.NewGuid());
        var thirdUserId = new UserId("THIRD-USER-ID");
        var gameId = Guid.NewGuid();
        var player1Id = Guid.NewGuid();
        var player2Id = Guid.NewGuid();

        using (var dbContext = await DbContextFactory.CreateDbContextAsync())
        {
            var otherVisitor = new VisitorDTO { Id = otherVisitorId.Value, Name = "Other Visitor" };
            var yetAnotherVisitor = new VisitorDTO { Id = Guid.NewGuid(), Name = "Yet Another Visitor" };
            await dbContext.Visitors.AddAsync(ObjectMother.VisitorUser);
            await dbContext.Visitors.AddAsync(otherVisitor);
            await dbContext.Visitors.AddAsync(yetAnotherVisitor);

            var gameDto = new GameDTO
            {
                Id = gameId,
                UserId = null, // Owned by visitor, not user
                VisitorId = otherVisitorId.Value,
                ColourScheme = ColourScheme.Original,
                AtomShape = AtomShape.Round,
                Board = ObjectMother.BoardDTO(),
                Move = 1,
                Round = 1,
                IsActive = true,
                Rng = new RngDTO { Seed = 1, Iterations = 0 },
                CreatedDateUtc = ObjectMother.CreatedDateUtc,
                LastUpdatedDateUtc = ObjectMother.LastUpdatedDateUtc
            };

            var player1 = CreatePlayerDto(player1Id, 1, userId, isActive: true);
            player1.Game = gameDto;
            gameDto.Players.Add(player1);

            var player2 = CreatePlayerDto(player2Id, 2, userId: thirdUserId, visitorId: new VisitorId(yetAnotherVisitor.Id), isActive: false);
            player2.Game = gameDto;
            gameDto.Players.Add(player2);

            await dbContext.Games.AddAsync(gameDto);
            await dbContext.SaveChangesAsync();
        }

        var handler = new DeleteUserDataRequestHandler(DbContextFactory, _identityDbContextFactory);
        await handler.Handle(new DeleteUserDataRequest(ObjectMother.VisitorId, userId), CancellationToken.None);

        using (var dbContext = await DbContextFactory.CreateDbContextAsync())
        {
            var game = await dbContext.Games.FindAsync(gameId);

            await Assert.That(game).IsNotNull();
            var player2FromDb = game!.Players.Single(p => p.Id == player2Id);
            await Assert.That(player2FromDb.UserId).IsEqualTo(thirdUserId.Id);
        }
    }

    [Test]
    public async Task ShouldHandleUserWithNoGames()
    {
        var userId = ObjectMother.UserId;
        var handler = new DeleteUserDataRequestHandler(DbContextFactory, _identityDbContextFactory);

        // Should not throw
        await handler.Handle(new DeleteUserDataRequest(ObjectMother.VisitorId, userId), CancellationToken.None);
    }

    [Test]
    public async Task ShouldHandleUserWithOnlyOwnedGames()
    {
        var userId = ObjectMother.UserId;
        var game1Id = Guid.NewGuid();
        var game2Id = Guid.NewGuid();

        using (var dbContext = await DbContextFactory.CreateDbContextAsync())
        {
            var game1 = CreateGameDtoOwnedByUser(game1Id, userId);
            var game2 = CreateGameDtoOwnedByUser(game2Id, userId);

            await dbContext.Visitors.AddAsync(ObjectMother.VisitorUser);
            await dbContext.Games.AddAsync(game1);
            await dbContext.Games.AddAsync(game2);
            await dbContext.SaveChangesAsync();
        }

        var handler = new DeleteUserDataRequestHandler(DbContextFactory, _identityDbContextFactory);
        await handler.Handle(new DeleteUserDataRequest(ObjectMother.VisitorId, userId), CancellationToken.None);

        using (var dbContext = await DbContextFactory.CreateDbContextAsync())
        {
            var deletedGame1 = await dbContext.Games.FindAsync(game1Id);
            var deletedGame2 = await dbContext.Games.FindAsync(game2Id);

            await Assert.That(deletedGame1).IsNull();
            await Assert.That(deletedGame2).IsNull();
        }
    }

    [Test]
    public async Task ShouldDeleteUserFromIdentityDatabase()
    {
        var userId = ObjectMother.UserId;

        using var identityDbContext = await _identityDbContextFactory.CreateDbContextAsync();
        var user = new ApplicationUser { Id = userId.Id, UserName = "testuser", Email = "test@example.com", Name = "Test User" };
        await identityDbContext.Users.AddAsync(user);
        await identityDbContext.SaveChangesAsync();

        var handler = new DeleteUserDataRequestHandler(DbContextFactory, _identityDbContextFactory);
        await handler.Handle(new DeleteUserDataRequest(ObjectMother.VisitorId, userId), CancellationToken.None);

        using var verifyDbContext = await _identityDbContextFactory.CreateDbContextAsync();
        var deletedUser = await verifyDbContext.FindAsync<ApplicationUser>(userId.Id);

        await Assert.That(deletedUser).IsNull();
    }

    [Test]
    public async Task ShouldNotDeleteUserOwnedGamesWhenUserIdIsNull()
    {
        var userId = ObjectMother.UserId;
        var otherVisitorId = new VisitorId(Guid.NewGuid());
        var gameId = Guid.NewGuid();

        using (var dbContext = await DbContextFactory.CreateDbContextAsync())
        {
            var otherVisitor = new VisitorDTO { Id = otherVisitorId.Value, Name = "Other Visitor" };
            await dbContext.Visitors.AddAsync(ObjectMother.VisitorUser);
            await dbContext.Visitors.AddAsync(otherVisitor);

            // Game owned by user but with a different visitor
            var gameDto = new GameDTO
            {
                Id = gameId,
                UserId = userId.Id,
                VisitorId = otherVisitorId.Value,
                ColourScheme = ColourScheme.Original,
                AtomShape = AtomShape.Round,
                Board = ObjectMother.BoardDTO(),
                Move = 1,
                Round = 1,
                IsActive = true,
                Rng = new RngDTO { Seed = 1, Iterations = 0 },
                CreatedDateUtc = ObjectMother.CreatedDateUtc,
                LastUpdatedDateUtc = ObjectMother.LastUpdatedDateUtc
            };

            var player1 = CreatePlayerDto(Guid.NewGuid(), 1, userId);
            player1.Game = gameDto;
            gameDto.Players.Add(player1);

            var player2 = CreatePlayerDto(Guid.NewGuid(), 2);
            player2.Game = gameDto;
            gameDto.Players.Add(player2);

            await dbContext.Games.AddAsync(gameDto);
            await dbContext.SaveChangesAsync();
        }

        var handler = new DeleteUserDataRequestHandler(DbContextFactory, _identityDbContextFactory);
        // Call with null UserId but different visitor - should not delete user's game
        await handler.Handle(new DeleteUserDataRequest(ObjectMother.VisitorId, null), CancellationToken.None);

        using (var dbContext = await DbContextFactory.CreateDbContextAsync())
        {
            var game = await dbContext.Games.FindAsync(gameId);
            await Assert.That(game).IsNotNull();
        }
    }

    [Test]
    public async Task ShouldNotClearUserIdFromPlayerEntriesWhenUserIdIsNull()
    {
        var userId = ObjectMother.UserId;
        var otherVisitorId = new VisitorId(Guid.NewGuid());
        var gameId = Guid.NewGuid();
        var player1Id = Guid.NewGuid();

        using (var dbContext = await DbContextFactory.CreateDbContextAsync())
        {
            var otherVisitor = new VisitorDTO { Id = otherVisitorId.Value, Name = "Other Visitor" };
            await dbContext.Visitors.AddAsync(ObjectMother.VisitorUser);
            await dbContext.Visitors.AddAsync(otherVisitor);

            var gameDto = new GameDTO
            {
                Id = gameId,
                UserId = null,
                VisitorId = otherVisitorId.Value,
                ColourScheme = ColourScheme.Original,
                AtomShape = AtomShape.Round,
                Board = ObjectMother.BoardDTO(),
                Move = 1,
                Round = 1,
                IsActive = true,
                Rng = new RngDTO { Seed = 1, Iterations = 0 },
                CreatedDateUtc = ObjectMother.CreatedDateUtc,
                LastUpdatedDateUtc = ObjectMother.LastUpdatedDateUtc
            };

            var player = CreatePlayerDto(player1Id, 1, userId);
            player.AbbreviatedName = "USR";
            player.Game = gameDto;
            gameDto.Players.Add(player);

            var player2 = CreateCpuPlayerDto(Guid.NewGuid(), 2);
            player2.Game = gameDto;
            gameDto.Players.Add(player2);

            await dbContext.Games.AddAsync(gameDto);
            await dbContext.SaveChangesAsync();
        }

        var handler = new DeleteUserDataRequestHandler(DbContextFactory, _identityDbContextFactory);
        await handler.Handle(new DeleteUserDataRequest(ObjectMother.VisitorId, null), CancellationToken.None);

        using (var dbContext = await DbContextFactory.CreateDbContextAsync())
        {
            var game = await dbContext.Games.FindAsync(gameId);

            await Assert.That(game).IsNotNull();
            var playerFromDb = game!.Players.Single(p => p.Id == player1Id);
            await Assert.That(playerFromDb.UserId).IsEqualTo(userId.Id);
            await Assert.That(playerFromDb.VisitorId).IsNull();
            await Assert.That(playerFromDb.AbbreviatedName).IsEqualTo("USR");
        }
    }

    [Test]
    public async Task ShouldStillDeleteVisitorOwnedGamesWhenUserIdIsNull()
    {
        var visitorId = ObjectMother.VisitorId;
        var gameId = Guid.NewGuid();

        using (var dbContext = await DbContextFactory.CreateDbContextAsync())
        {
            var gameDto = new GameDTO
            {
                Id = gameId,
                UserId = null,
                VisitorId = visitorId.Value,
                ColourScheme = ColourScheme.Original,
                AtomShape = AtomShape.Round,
                Board = ObjectMother.BoardDTO(),
                Move = 1,
                Round = 1,
                IsActive = true,
                Rng = new RngDTO { Seed = 1, Iterations = 0 },
                CreatedDateUtc = ObjectMother.CreatedDateUtc,
                LastUpdatedDateUtc = ObjectMother.LastUpdatedDateUtc
            };

            var player1 = CreatePlayerDto(Guid.NewGuid(), 1, isActive: true);
            player1.Game = gameDto;
            gameDto.Players.Add(player1);

            var player2 = CreateCpuPlayerDto(Guid.NewGuid(), 2);
            player2.Game = gameDto;
            gameDto.Players.Add(player2);

            await dbContext.Visitors.AddAsync(ObjectMother.VisitorUser);
            await dbContext.Games.AddAsync(gameDto);
            await dbContext.SaveChangesAsync();
        }

        var handler = new DeleteUserDataRequestHandler(DbContextFactory, _identityDbContextFactory);
        await handler.Handle(new DeleteUserDataRequest(visitorId, null), CancellationToken.None);

        using (var dbContext = await DbContextFactory.CreateDbContextAsync())
        {
            var deletedGame = await dbContext.Games.FindAsync(gameId);
            await Assert.That(deletedGame).IsNull();
        }
    }

    [Test]
    public async Task ShouldStillDeleteVisitorWhenUserIdIsNull()
    {
        var visitorId = ObjectMother.VisitorId;

        using (var dbContext = await DbContextFactory.CreateDbContextAsync())
        {
            await dbContext.Visitors.AddAsync(ObjectMother.VisitorUser);
            await dbContext.SaveChangesAsync();
        }

        var handler = new DeleteUserDataRequestHandler(DbContextFactory, _identityDbContextFactory);
        await handler.Handle(new DeleteUserDataRequest(visitorId, null), CancellationToken.None);

        using (var dbContext = await DbContextFactory.CreateDbContextAsync())
        {
            var deletedVisitor = await dbContext.Visitors.FindAsync(visitorId.Value);
            await Assert.That(deletedVisitor).IsNull();
        }
    }

    [Test]
    public async Task ShouldNotDeleteUserFromIdentityDatabaseWhenUserIdIsNull()
    {
        var userId = ObjectMother.UserId;

        using var identityDbContext = await _identityDbContextFactory.CreateDbContextAsync();
        var user = new ApplicationUser { Id = userId.Id, UserName = "testuser", Email = "test@example.com", Name = "Test User" };
        await identityDbContext.Users.AddAsync(user);
        await identityDbContext.SaveChangesAsync();

        var handler = new DeleteUserDataRequestHandler(DbContextFactory, _identityDbContextFactory);
        await handler.Handle(new DeleteUserDataRequest(ObjectMother.VisitorId, null), CancellationToken.None);

        using var verifyDbContext = await _identityDbContextFactory.CreateDbContextAsync();
        var userStillExists = await verifyDbContext.FindAsync<ApplicationUser>(userId.Id);

        await Assert.That(userStillExists).IsNotNull();
    }

    [Test]
    public async Task ShouldSetAbbreviatedNameToNullForPlayerEntriesInNonOwnedGames()
    {
        var userId = ObjectMother.UserId;
        var otherVisitorId = new VisitorId(Guid.NewGuid());
        var gameId = Guid.NewGuid();
        var player1Id = Guid.NewGuid();

        using (var dbContext = await DbContextFactory.CreateDbContextAsync() ?? throw new InvalidOperationException("Failed to create database context"))
        {
            var otherVisitor = new VisitorDTO { Id = otherVisitorId.Value, Name = "Other Visitor" };
            await dbContext.Visitors.AddAsync(ObjectMother.VisitorUser);
            await dbContext.Visitors.AddAsync(otherVisitor);

            var gameDto = new GameDTO
            {
                Id = gameId,
                UserId = null,
                VisitorId = otherVisitorId.Value,
                ColourScheme = ColourScheme.Original,
                AtomShape = AtomShape.Round,
                Board = ObjectMother.BoardDTO(),
                Move = 1,
                Round = 1,
                IsActive = true,
                Rng = new RngDTO { Seed = 1, Iterations = 0 },
                CreatedDateUtc = ObjectMother.CreatedDateUtc,
                LastUpdatedDateUtc = ObjectMother.LastUpdatedDateUtc
            };

            var player1 = CreatePlayerDto(player1Id, 1, userId);
            player1.AbbreviatedName = "ABC";
            player1.Game = gameDto;
            gameDto.Players.Add(player1);

            var player2 = CreateCpuPlayerDto(Guid.NewGuid(), 2);
            player2.Game = gameDto;
            gameDto.Players.Add(player2);

            await dbContext.Games.AddAsync(gameDto);
            await dbContext.SaveChangesAsync();
        }

        var handler = new DeleteUserDataRequestHandler(DbContextFactory, _identityDbContextFactory);
        await handler.Handle(new DeleteUserDataRequest(ObjectMother.VisitorId, userId), CancellationToken.None);

        using (var dbContext = await DbContextFactory.CreateDbContextAsync() ?? throw new InvalidOperationException("Failed to create database context"))
        {
            var game = await dbContext.Games.FindAsync(gameId);

            await Assert.That(game).IsNotNull();
            var playerFromDb = game!.Players.Single(p => p.Id == player1Id);
            await Assert.That(playerFromDb.AbbreviatedName).IsNull();
        }
    }

    [Test]
    public async Task ShouldSetAbbreviatedNameToNullForAllPlayerEntriesWhenUserIsDeleted()
    {
        var userId = ObjectMother.UserId;
        var game1Id = Guid.NewGuid();
        var game2Id = Guid.NewGuid();

        using (var dbContext = await DbContextFactory.CreateDbContextAsync() ?? throw new InvalidOperationException("Failed to create database context"))
        {
            var otherVisitorId1 = Guid.NewGuid();
            var otherVisitorId2 = Guid.NewGuid();
            await dbContext.Visitors.AddAsync(ObjectMother.VisitorUser);
            await dbContext.Visitors.AddAsync(new VisitorDTO { Id = otherVisitorId1, Name = "Visitor 1" });
            await dbContext.Visitors.AddAsync(new VisitorDTO { Id = otherVisitorId2, Name = "Visitor 2" });

            var game1 = CreateGameDtoOwnedByVisitor(game1Id, otherVisitorId1);
            var player1 = CreatePlayerDto(Guid.NewGuid(), 1, userId, isActive: true);
            player1.AbbreviatedName = "PLY";
            player1.Game = game1;
            game1.Players.Add(player1);
            var player2 = CreateCpuPlayerDto(Guid.NewGuid(), 2);
            player2.Game = game1;
            game1.Players.Add(player2);

            var game2 = CreateGameDtoOwnedByVisitor(game2Id, otherVisitorId2);
            var player3 = CreatePlayerDto(Guid.NewGuid(), 1, userId, isActive: false);
            player3.AbbreviatedName = "ABC";
            player3.Game = game2;
            game2.Players.Add(player3);
            var player4 = CreateCpuPlayerDto(Guid.NewGuid(), 2);
            player4.Game = game2;
            game2.Players.Add(player4);

            await dbContext.Games.AddRangeAsync(game1, game2);
            await dbContext.SaveChangesAsync();
        }

        var handler = new DeleteUserDataRequestHandler(DbContextFactory, _identityDbContextFactory);
        await handler.Handle(new DeleteUserDataRequest(ObjectMother.VisitorId, userId), CancellationToken.None);

        using (var dbContext = await DbContextFactory.CreateDbContextAsync() ?? throw new InvalidOperationException("Failed to create database context"))
        {
            var players = await dbContext.Players.Where(p => p.AbbreviatedName == null && p.PlayerTypeId == PlayerType.Human).ToListAsync();
            await Assert.That(players.Count).EqualTo(2);
        }
    }

    [Test]
    public async Task ShouldNotChangeAbbreviatedNameForOtherPlayersInNonOwnedGames()
    {
        var userId = ObjectMother.UserId;
        var otherVisitorId = new VisitorId(Guid.NewGuid());
        var thirdUserId = new UserId("THIRD-USER-ID");
        var gameId = Guid.NewGuid();
        var player1Id = Guid.NewGuid();
        var player2Id = Guid.NewGuid();

        using (var dbContext = await DbContextFactory.CreateDbContextAsync() ?? throw new InvalidOperationException("Failed to create database context"))
        {
            var otherVisitor = new VisitorDTO { Id = otherVisitorId.Value, Name = "Other Visitor" };
            var yetAnotherVisitor = new VisitorDTO { Id = Guid.NewGuid(), Name = "Yet Another Visitor" };
            await dbContext.Visitors.AddAsync(ObjectMother.VisitorUser);
            await dbContext.Visitors.AddAsync(otherVisitor);
            await dbContext.Visitors.AddAsync(yetAnotherVisitor);

            var gameDto = new GameDTO
            {
                Id = gameId,
                UserId = null, // Owned by visitor, not user
                VisitorId = otherVisitorId.Value,
                ColourScheme = ColourScheme.Original,
                AtomShape = AtomShape.Round,
                Board = ObjectMother.BoardDTO(),
                Move = 1,
                Round = 1,
                IsActive = true,
                Rng = new RngDTO { Seed = 1, Iterations = 0 },
                CreatedDateUtc = ObjectMother.CreatedDateUtc,
                LastUpdatedDateUtc = ObjectMother.LastUpdatedDateUtc
            };

            var player1 = CreatePlayerDto(player1Id, 1, userId, isActive: true);
            player1.AbbreviatedName = "USR";
            player1.Game = gameDto;
            gameDto.Players.Add(player1);

            var player2 = CreatePlayerDto(player2Id, 2, userId: thirdUserId, visitorId: new VisitorId(yetAnotherVisitor.Id), isActive: false);
            player2.AbbreviatedName = "OTH";
            player2.Game = gameDto;
            gameDto.Players.Add(player2);

            await dbContext.Games.AddAsync(gameDto);
            await dbContext.SaveChangesAsync();
        }

        var handler = new DeleteUserDataRequestHandler(DbContextFactory, _identityDbContextFactory);
        await handler.Handle(new DeleteUserDataRequest(ObjectMother.VisitorId, userId), CancellationToken.None);

        using (var dbContext = await DbContextFactory.CreateDbContextAsync() ?? throw new InvalidOperationException("Failed to create database context"))
        {
            var game = await dbContext.Games.FindAsync(gameId);

            await Assert.That(game).IsNotNull();
            var otherPlayer = game!.Players.Single(p => p.Id == player2Id);
            await Assert.That(otherPlayer.AbbreviatedName).IsEqualTo("OTH");
        }
    }

    protected override async Task SetupInternal()
    {
        _identityDbContextFactory = CreateIdentityDbContextFactory();
    }

    private static GameDTO CreateGameDtoOwnedByVisitor(Guid gameId, Guid otherVisitorId)
    {
        return new GameDTO
        {
            Id = gameId,
            UserId = null,
            VisitorId = otherVisitorId,
            ColourScheme = ColourScheme.Original,
            AtomShape = AtomShape.Round,
            Board = ObjectMother.BoardDTO(),
            Move = 1,
            Round = 1,
            IsActive = true,
            Rng = new RngDTO { Seed = 1, Iterations = 0 },
            CreatedDateUtc = ObjectMother.CreatedDateUtc,
            LastUpdatedDateUtc = ObjectMother.LastUpdatedDateUtc
        };
    }

    static GameDTO CreateGameDtoOwnedByUser(Guid gameId, UserId userId)
    {
        var game = new GameDTO
        {
            Id = gameId,
            UserId = userId.Id,
            VisitorId = ObjectMother.VisitorId.Value,
            ColourScheme = ColourScheme.Original,
            AtomShape = AtomShape.Round,
            Board = ObjectMother.BoardDTO(),
            Move = 1,
            Round = 1,
            IsActive = true,
            Rng = new RngDTO { Seed = 1, Iterations = 0 },
            CreatedDateUtc = ObjectMother.CreatedDateUtc,
            LastUpdatedDateUtc = ObjectMother.LastUpdatedDateUtc
        };

        var player1 = CreatePlayerDto(Guid.NewGuid(), 1, userId, isActive: true);
        player1.Game = game;
        game.Players.Add(player1);

        var player2 = CreateCpuPlayerDto(Guid.NewGuid(), 2);
        player2.Game = game;
        game.Players.Add(player2);

        return game;
    }

    static PlayerDTO CreatePlayerDto(Guid playerId, int number, UserId? userId = null, VisitorId? visitorId = null, bool isActive = false)
    {
        return new PlayerDTO
        {
            Id = playerId,
            Number = number,
            PlayerTypeId = PlayerType.Human,
            VisitorId = visitorId?.Value ?? ObjectMother.VisitorId.Value,
            Game = null!,
            IsActive = isActive,
            UserId = userId?.Id
        };
    }

    static PlayerDTO CreateCpuPlayerDto(Guid playerId, int number)
    {
        return new PlayerDTO
        {
            Id = playerId,
            Number = number,
            PlayerTypeId = PlayerType.CPU_Easy,
            VisitorId = null,
            UserId = null,
            Game = null!,
            IsActive = false
        };
    }
}

