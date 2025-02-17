using Atoms.Core.ExtensionMethods;
using Atoms.Core.Interfaces;
using Atoms.Core.ValueObjects;
using static Atoms.Core.Entities.Game;

namespace Atoms.Core.DTOs;

public class GameDTO
{
    public required Guid Id { get; init; }
    public required Guid LocalStorageId { get; init; }
    public required ColourScheme ColourScheme { get; init; }
    public required AtomShape AtomShape { get; init; }
    public ICollection<PlayerDTO> Players { get; } = [];
    public required BoardDTO Board { get; set; }
    public required int Move { get; set; }
    public required int Round { get; set; }
    public required bool IsActive { get; set; }
    public required RngDTO Rng { get; set; }

    public static GameDTO FromEntity(Game game, StorageId storageId)
    {
        var gameDto = new GameDTO
        {
            Id = game.Id,
            LocalStorageId = storageId.Value,
            ColourScheme = game.ColourScheme,
            AtomShape = game.AtomShape,
            Board = BoardDTO.FromEntity(game.Board),
            IsActive = !game.HasWinner,
            Move = game.Move,
            Round = game.Round,
            Rng = RngDTO.FromEntity(game.Rng)
        };

        foreach (var player in game.Players)
        {
            gameDto.Players.Add(new PlayerDTO
            {
                Id = player.Id,
                Number = player.Number,
                Game = gameDto,
                Type = player.Type,
                IsWinner = game.Winner == player,
                UserId = player.UserId
            });
        }

        return gameDto;
    }

    public Game ToEntity(CreateRng rngFactory, CreatePlayerStrategy playerStrategyFactory)
    {
        var rng = rngFactory.Invoke(Rng.Seed, Rng.Iterations);

        List<Player> players =
        [.. Players
                .OrderBy(dto => dto.Number)
                .Select(dto =>
                        new Player(
                            dto.Id,
                            dto.Number,
                            dto.Type,
                            dto.UserId,
                            playerStrategyFactory.Invoke(dto.Type, rng)))
        ];

        return new Game(
            Id,
            Constants.Rows, Constants.Columns,
            players,
            players[(Move - 1) % players.Count],
            ColourScheme,
            AtomShape,
            rng,
            Board.ToEntity(),
            Move,
            Round);
    }

    public void UpdateFromEntity(Game game)
    {
        Move = game.Move;
        Round = game.Round;
        Rng = RngDTO.FromEntity(game.Rng);
        Board = BoardDTO.FromEntity(game.Board);
        IsActive = !game.HasWinner;

        foreach (var player in game.Players)
        {
            var playerDto = Players.First(p => p.Number == player.Number);

            playerDto.IsWinner = game.HasWinner && !player.IsDead;
        }
    }
}

public class PlayerDTO
{
    public required Guid Id { get; init; }
    public required int Number { get; init; }
    public required PlayerType Type { get; init; }
    public Guid? LocalStorageId { get; set; }
    public string? UserId { get; init; }
    public bool IsWinner { get; set; }
    public Guid GameId { get; init; }
    public GameDTO Game { get; set; } = default!;
}

public class RngDTO
{
    public required int Seed { get; init; }
    public required int Iterations { get; set; }

    public IRandomNumberGenerator ToEntity(
        Func<int, int, IRandomNumberGenerator> rngFactory)
        => rngFactory.Invoke(Seed, Iterations);

    public static RngDTO FromEntity(IRandomNumberGenerator rng) =>
        new() { Seed = rng.Seed, Iterations = rng.Iterations };
}

public class BoardDTO
{
    const char CellSeparator = ',';
    const char CellItemSeparator = '-';

    public string Data { get; set; } = default!;

    public static BoardDTO FromEntity(GameBoard board)
    {
        var cellCsv = board.Cells
            .Where(cell => cell.Player is not null)
            .Select(cell =>
                string.Join(
                    CellItemSeparator,
                    cell.Row, cell.Column, cell.Player, cell.Atoms))
            .ToCsv(CellSeparator);

        return new() { Data = cellCsv };
    }

    public List<GameBoard.CellState> ToEntity()
    {
        return [.. Data.Split(CellSeparator, StringSplitOptions.RemoveEmptyEntries)
                       .Select(cellData =>
                        {
                            var cellItemData = cellData.Split(CellItemSeparator);

                            return new GameBoard.CellState(
                                int.Parse(cellItemData[0]),
                                int.Parse(cellItemData[1]),
                                int.Parse(cellItemData[2]),
                                int.Parse(cellItemData[3]));
                        }) ];
    }
}
