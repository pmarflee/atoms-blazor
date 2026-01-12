using Atoms.Core.Identity;
using Atoms.Core.Services;
using Atoms.Core.ValueObjects;
using Atoms.UseCases.Invites.ReadInviteCode;
using Microsoft.Extensions.Logging;

using static Atoms.Core.Entities.Game;

namespace Atoms.UnitTests;

internal static class ObjectMother
{
    static ObjectMother()
    {
        GameMenuOptions = CreateGameMenuOptions(Atoms.Core.Constants.MinPlayers);
    }

    public const int Rows = 6;
    public const int Columns = 10;

    public static readonly Guid GameId = new("5BD0E94D-ED21-4679-8B31-E2C70945C8B4");
    public static readonly StorageId LocalStorageId = new(Guid.Parse("22D05F6C-DE9B-4B70-81B0-A54E0E83DA6D"));
    public static readonly UserId UserId = new("7B452FD8-C32C-497A-BC20-2190C1244B9E");
    public static readonly string Username = "David";
    public static readonly UserIdentity UserIdentity = new(UserId, Username);

    public static LocalStorageUserDTO LocalStorageUser => new() { Id = LocalStorageId.Value, Name = Username };

    public static readonly Guid Player1Id = new("FE0FA471-AC98-4D1B-825B-4DDF64122022");
    public static readonly Guid Player2Id = new("08C5B9A7-0B0C-4E2F-9741-0FE822093901");

    public static readonly DateTime CreatedDateUtc = new(2025, 3, 12, 23, 14, 0);
    public static readonly DateTime LastUpdatedDateUtc = new(2025, 3, 12, 23, 30, 0);
    public static readonly DateTime NewLastUpdatedDateUtc = new(2025, 3, 22, 18, 15, 0);

    public static Invite Invite = new(Player1Id);

    public const string BaseUrl = "https://www.atoms.com";

    public static readonly GameMenuOptions GameMenuOptions;

    public static GameMenuOptions CreateGameMenuOptions(int numberOfPlayers)
        => new()
        {
            NumberOfPlayers = numberOfPlayers,
            Players = [..
                Enumerable.Range(0, numberOfPlayers)
                    .Select(i => new GameMenuOptions.Player
                    {
                        Number = i + 1,
                        Type = PlayerType.Human
                    })
            ],
            ColourScheme = ColourScheme.Original,
            AtomShape = AtomShape.Round,
            HasSound = true
        };

    public static Game Game(List<Player>? players = null,
                            int? active = 1,
                            List<CellState>? cells = null,
                            int move = 1,
                            int round = 1,
                            UserId? userId = null,
                            StorageId? localStorageId = null,
                            DateTime? lastUpdatedDateUtc = null)
    {
        players ??=
        [
            new (Player1Id, 1, PlayerType.Human, localStorageId: LocalStorageId),
            new (Player2Id, 2, PlayerType.Human),
        ];

        var activePlayer = players.First(p => p.Number == active);
        var seed = 1;
        var random = new Random(seed);
        var rng = new RandomNumberGenerator(random, seed, 0);

        return new Game(GameId, Rows, Columns, players, activePlayer,
                        ColourScheme.Original, AtomShape.Round,
                        rng,
                        localStorageId ?? LocalStorageId,
                        CreatedDateUtc,
                        lastUpdatedDateUtc ?? LastUpdatedDateUtc,
                        cells, move, round,
                        userId ?? UserId);
    }

    public static Player CreateHumanPlayer(
        Guid id, int number,
        UserId? userId = null,
        StorageId? localStorageId = null,
        string? name = null)
    {
        return new(
            id, number, PlayerType.Human,
            userId: userId,
            localStorageId: localStorageId,
            name: name);
    }

    public static Player CreateCPUPlayer(Guid id,
                                         int number,
                                         PlayerType? type = null)
    {
        type ??= PlayerType.CPU_Easy;

        return new(
            id, number, type,
            strategy: CreatePlayerStrategy(type, CreateRng(0, 0)));
    }

    public static GameDTO GameDTO(
        List<PlayerDTO>? players = null,
        int move = 1,
        int round = 1,
        BoardDTO? board = null,
        DateTime? lastUpdatedDateUtc = null,
        bool isActive = true)
    {
        var gameDto = new GameDTO
        {
            Id = GameId,
            UserId = UserId.Id,
            LocalStorageUserId = LocalStorageId.Value,
            ColourScheme = ColourScheme.Original,
            AtomShape = AtomShape.Round,
            Board = board ?? BoardDTO(),
            Move = move,
            Round = round,
            IsActive = isActive,
            Rng = new RngDTO { Seed = 1, Iterations = 0 },
            CreatedDateUtc = CreatedDateUtc,
            LastUpdatedDateUtc = lastUpdatedDateUtc ?? LastUpdatedDateUtc
        };

        players ??=
            [
                new PlayerDTO
                {
                    Id = Player1Id,
                    Number = 1,
                    PlayerTypeId = PlayerType.Human,
                    LocalStorageUserId = LocalStorageId.Value,
                    Game = gameDto,
                    IsActive = true
                },
                new PlayerDTO
                {
                    Id = Player2Id,
                    Number = 2,
                    PlayerTypeId = PlayerType.Human,
                    Game = gameDto
                }
            ];

        players.ForEach(player =>
        {
            player.Game = gameDto;

            gameDto.Players.Add(player);
        });

        return gameDto;
    }

    public static BoardDTO BoardDTO(string data = "")
    {
        return new BoardDTO { Data = data };
    }

    public static IRandomNumberGenerator CreateRng(int _, int __)
    {
        var expectations = new IRandomNumberGeneratorCreateExpectations();

        expectations.Setups.Seed.Gets().ReturnValue(1);
        expectations.Setups.Iterations.Gets().ReturnValue(0);

        return expectations.Instance();
    }

    public static IPlayerStrategy? CreatePlayerStrategy(
        PlayerType type, IRandomNumberGenerator _)
    {
        return type.Name == PlayerType.Human.Name
            ? null
            : new IPlayerStrategyCreateExpectations().Instance();
    }

    public static ApplicationUser CreateApplicationUser(UserId userId)
    {
        return new ApplicationUser { Id = userId.Id };
    }

    public static ValueTask<ApplicationUser> GetUserById(UserId userId)
    {
        return ValueTask.FromResult(CreateApplicationUser(userId));
    }

    public static ValueTask<LocalStorageUserDTO> GetLocalStorageUserById(StorageId storageId)
    {
        return ValueTask.FromResult(new LocalStorageUserDTO { Id = storageId.Value });
    }

    public sealed class MockReadInviteCodeRequestLogger : ILogger<ReadInviteCodeRequestHandler>
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
        }
    }
}
