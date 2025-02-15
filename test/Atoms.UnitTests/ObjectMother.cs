using Atoms.Core.DTOs;
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

    public static readonly Guid Player1Id = new("FE0FA471-AC98-4D1B-825B-4DDF64122022");
    public static readonly Guid Player2Id = new("08C5B9A7-0B0C-4E2F-9741-0FE822093901");

    public static readonly GameMenuOptions GameMenuOptions = new(2, 4);

    public static Game Game(List<Player>? players = null,
                            int? active = 1,
                            List<CellState>? cells = null,
                            int move = 1,
                            int round = 1)
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
                        rng, cells, move, round);
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
}
