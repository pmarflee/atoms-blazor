using static Atoms.Core.Entities.Game;

namespace Atoms.Core.DTOs;

public class GameInfoDTO
{
    public Guid Id { get; init; }
    public int Move { get; init; }
    public int Round { get; init; }
    public string? Opponents { get; init; }
    public bool IsActive { get; init; }
    public string? Winner { get; init; }
    public DateTime CreatedDateUtc { get; init; }
    public DateTime LastUpdatedDateUtc { get; init; }
}

public class PlayerTypeDTO
{ 
    public required PlayerType Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
}

public class LocalStorageUserDTO
{
    public required Guid Id { get; init; }
    public string? Name { get; set; }

    public ICollection<GameDTO> Games { get; } = [];
    public ICollection<PlayerDTO> Players { get; } = [];
}

public class GameDTO
{
    public required Guid Id { get; init; }
    public string? UserId { get; init; }
    public required Guid LocalStorageUserId { get; init; }
    public LocalStorageUserDTO LocalStorageUser { get; init; } = default!;
    public required ColourScheme ColourScheme { get; init; }
    public required AtomShape AtomShape { get; init; }
    public ICollection<PlayerDTO> Players { get; } = new SortedSet<PlayerDTO>(new PlayerDTOSortComparer());
    public required BoardDTO Board { get; set; }
    public required int Move { get; set; }
    public required int Round { get; set; }
    public required bool IsActive { get; set; }
    public required RngDTO Rng { get; set; }
    public DateTime CreatedDateUtc { get; set; }
    public DateTime LastUpdatedDateUtc { get; set; }

    public static GameDTO FromEntity(Game game)
    {
        var gameDto = new GameDTO
        {
            Id = game.Id,
            UserId = game.UserId?.Id,
            LocalStorageUserId = game.LocalStorageId.Value,
            ColourScheme = game.ColourScheme,
            AtomShape = game.AtomShape,
            Board = BoardDTO.FromEntity(game.Board),
            IsActive = !game.HasWinner,
            Move = game.Move,
            Round = game.Round,
            Rng = RngDTO.FromEntity(game.Rng),
            CreatedDateUtc = game.CreatedDateUtc,
            LastUpdatedDateUtc = game.LastUpdatedDateUtc
        };

        foreach (var player in game.Players)
        {
            gameDto.Players.Add(new PlayerDTO
            {
                Id = player.Id,
                Number = player.Number,
                Game = gameDto,
                PlayerTypeId = player.Type,
                IsWinner = game.Winner == player,
                UserId = player.UserId?.Id,
                AbbreviatedName = player.AbbreviatedName,
                LocalStorageUserId = player.LocalStorageId?.Value,
                IsActive = game.ActivePlayer == player
            });
        }

        return gameDto;
    }

    public async Task<Game> ToEntity(CreateRng rngFactory,
                                     CreatePlayerStrategy playerStrategyFactory,
                                     GetUserById getUserById,
                                     GetLocalStorageUserById getLocalStorageUserById)
    {
        var rng = rngFactory.Invoke(Rng.Seed, Rng.Iterations);

        List<Player> players = [];
        Player? activePlayer = null;

        var numberOfActivePlayers = Players.Count(p => p.IsActive);

        if (numberOfActivePlayers != 1)
        {
            throw new Exception("Game needs to have a single active player");
        }

        foreach (var playerDto in Players.OrderBy(dto => dto.Number))
        {
            var user = playerDto.UserId != null
                ? (await getUserById.Invoke(playerDto.UserId!))
                : null;

            StorageId? localStorageId;
            LocalStorageUserDTO? localStorageUser;

            if (playerDto.LocalStorageUserId.HasValue)
            {
                localStorageId = (StorageId?)new StorageId(playerDto.LocalStorageUserId.Value);
                localStorageUser = user is null
                    ? await getLocalStorageUserById.Invoke(localStorageId!)
                    : null;
            }
            else
            {
                localStorageId = null;
                localStorageUser = null;
            }

            var player = new Player(playerDto.Id,
                                    playerDto.Number,
                                    playerDto.PlayerTypeId,
                                    playerDto.UserId,
                                    user?.Name ?? localStorageUser?.Name,
                                    playerDto.AbbreviatedName,
                                    playerStrategyFactory.Invoke(playerDto.PlayerTypeId, rng),
                                    localStorageId);

            if (playerDto.IsActive)
            {
                activePlayer = player;
            }

            players.Add(player);
        }

        if (activePlayer is null)
        {
            throw new Exception("Game does not have an active player");
        }

        var userId = UserId is not null
            ? new UserId(UserId)
            : null;

        return new Game(
            Id,
            Constants.Rows, Constants.Columns,
            players,
            activePlayer,
            ColourScheme,
            AtomShape,
            rng,
            new(LocalStorageUserId),
            CreatedDateUtc,
            LastUpdatedDateUtc,
            Board.ToEntity(),
            Move,
            Round,
            userId);
    }

    public void UpdateFromEntity(Game game, DateTime lastUpdatedDateUtc)
    {
        Move = game.Move;
        Round = game.Round;
        Rng = RngDTO.FromEntity(game.Rng);
        Board = BoardDTO.FromEntity(game.Board);
        IsActive = !game.HasWinner;
        LastUpdatedDateUtc = lastUpdatedDateUtc;

        foreach (var player in game.Players)
        {
            var playerDto = Players.First(p => p.Number == player.Number);

            playerDto.IsWinner = game.HasWinner && !player.IsDead;
            playerDto.UserId = player.UserId?.Id;
            playerDto.LocalStorageUserId = player.LocalStorageId?.Value;
            playerDto.IsActive = game.ActivePlayer == player;
        }

        game.MarkUpdated(LastUpdatedDateUtc);
    }
}

public class PlayerDTO
{
    public required Guid Id { get; init; }
    public required int Number { get; init; }
    public required PlayerType PlayerTypeId { get; init; }
    public Guid? LocalStorageUserId { get; set; }
    public LocalStorageUserDTO? LocalStorageUser { get; set; }
    public string? UserId { get; set; }
    public bool IsWinner { get; set; }
    public Guid GameId { get; init; }
    public string? AbbreviatedName { get; set; }
    public GameDTO Game { get; set; } = default!;
    public PlayerTypeDTO PlayerType { get; set; } = default!;
    public bool IsActive { get; set; }
}

class PlayerDTOSortComparer : IComparer<PlayerDTO>
{
    public int Compare(PlayerDTO? x, PlayerDTO? y)
    {
        if (x == null && y == null) return 0;
        if (x == null) return -1;
        if (y == null) return 1;

        return x.Number.CompareTo(y.Number);
    }
}

public class RngDTO
{
    public required int Seed { get; init; }
    public required int Iterations { get; set; }

    public IRandomNumberGenerator ToEntity(
        Func<int, int, IRandomNumberGenerator> rngFactory)
        => rngFactory.Invoke(Seed, Iterations);

    public RngDTO Clone() => new() { Seed = Seed, Iterations = Iterations };

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
