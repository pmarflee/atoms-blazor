using Atoms.Core.DTOs;
using Atoms.Core.Identity;
using Atoms.Core.Services;
using Atoms.Core.ValueObjects;
using static Atoms.Core.Entities.Game;

namespace Atoms.UnitTests;

internal static class ObjectMother
{
    public const int Rows = 6;
    public const int Columns = 10;

    public static readonly Guid GameId = new("5BD0E94D-ED21-4679-8B31-E2C70945C8B4");
    public static readonly StorageId LocalStorageId = new(Guid.Parse("22D05F6C-DE9B-4B70-81B0-A54E0E83DA6D"));
    public static readonly UserId UserId = new("7B452FD8-C32C-497A-BC20-2190C1244B9E");

    public static readonly Guid Player1Id = new("FE0FA471-AC98-4D1B-825B-4DDF64122022");
    public static readonly Guid Player2Id = new("08C5B9A7-0B0C-4E2F-9741-0FE822093901");

    public static Invite Invite = new(GameId, Player1Id);

    public const string BaseUrl = "https://www.atoms.com";

    public static readonly GameMenuOptions GameMenuOptions =
        new(GameId, 
            [
                new GameMenuOptions.Player 
                {
                    Id = Player1Id,
                    Number = 1,
                    Type = PlayerType.Human
                },
                new GameMenuOptions.Player
                {
                    Id = Player2Id,
                    Number = 2,
                    Type = PlayerType.Human
                }
            ],
            LocalStorageId,
            UserId);

    public static Game Game(List<Player>? players = null,
                            int? active = 1,
                            List<CellState>? cells = null,
                            int move = 1,
                            int round = 1,
                            UserId? userId = null,
                            StorageId? localStorageId = null)
    {
        players ??=
        [
            new (Player1Id, 1, PlayerType.Human),
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
                        cells, move, round,
                        userId ?? UserId);
    }

    public static Player CreateHumanPlayer(
        Guid id, int number, 
        ApplicationUser? applicationUser = null, 
        StorageId? localStorageId = null)
    {
        return new(
            id, number, PlayerType.Human, 
            user: applicationUser,
            localStorageId: localStorageId);
    }

    public static Player CreateCPUPlayer(Guid id, int number, PlayerType type = PlayerType.CPU_Easy)
    {
        return new(
            id, number, type,
            strategy: CreatePlayerStrategy(type, CreateRng(0, 0)));
    }

    public static GameDTO GameDTO(
        List<PlayerDTO>? players = null,
        int move = 1,
        int round = 1,
        BoardDTO? board = null)
    {
        var gameDto = new GameDTO
        {
            Id = GameId,
            UserId = UserId.Id,
            LocalStorageId = LocalStorageId.Value,
            ColourScheme = ColourScheme.Original,
            AtomShape = AtomShape.Round,
            Board = board ?? BoardDTO(),
            Move = move,
            Round = round,
            IsActive = true,
            Rng = new RngDTO { Seed = 1, Iterations = 0 }
        };

        players ??=
            [
                new PlayerDTO
                {
                    Id = Player1Id,
                    Number = 1,
                    Type = PlayerType.Human,
                    Game = gameDto
                },
                new PlayerDTO
                {
                    Id = Player2Id,
                    Number = 2,
                    Type = PlayerType.Human,
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
        return new IRandomNumberGeneratorCreateExpectations().Instance();
    }

    public static IPlayerStrategy? CreatePlayerStrategy(
        PlayerType type, IRandomNumberGenerator _)
    {
        return type switch
        {
            PlayerType.Human => null,
            _ => new IPlayerStrategyCreateExpectations().Instance()
        };
    }

    public static ApplicationUser CreateApplicationUser(UserId userId)
    {
        return new ApplicationUser { Id = userId.Id };
    }
}
