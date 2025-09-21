using MoreLinq;
using System.Diagnostics.CodeAnalysis;
using System.Text;

using LinqIndex = System.Linq.Enumerable;

namespace Atoms.Core.Entities;

public class Game
{
    public Guid Id { get; }
    public IReadOnlyList<Player> Players { get; }
    public ColourScheme ColourScheme { get; }
    public AtomShape AtomShape { get; }
    public int Rows => Board.Rows;
    public int Columns => Board.Columns;
    public GameBoard Board { get; }
    public Player ActivePlayer { get; private set; }
    public Player? Winner { get; private set; }
    public bool HasWinner => Winner != null;
    public int Move { get; private set; }
    public int Round { get; private set; }
    public IRandomNumberGenerator Rng { get; }
    public UserId? UserId { get; }
    public StorageId LocalStorageId { get; }
    public DateTime CreatedDateUtc { get; private set; }
    public DateTime LastUpdatedDateUtc { get; private set; }

    public HashSet<GameBoard.Cell> DangerCells =>
        [..from cell in Board.Cells
         where cell.IsFullyLoaded
         && cell.Player != ActivePlayer.Number
         from neighbour in Board.GetNeighbours(cell)
         where CanPlaceAtom(neighbour)
         && !neighbour.IsFullyLoaded
         select neighbour];

    public Game(Guid id,
                int rows,
                int columns,
                IReadOnlyList<Player> players,
                Player activePlayer,
                ColourScheme colourScheme,
                AtomShape atomShape,
                IRandomNumberGenerator rng,
                StorageId localStorageId,
                DateTime createdDateUtc,
                DateTime? lastUpdatedDateUtc = null,
                IEnumerable<GameBoard.CellState>? cells = null,
                int move = 1,
                int round = 1,
                UserId? userId = null)
    {
        if (!players.Contains(activePlayer))
        {
            throw new ArgumentException(
                "Player not found", nameof(activePlayer));
        }

        ColourScheme = colourScheme;
        AtomShape = atomShape;
        Board = new GameBoard(rows, columns, cells);
        Id = id;
        Players = players;
        ActivePlayer = activePlayer;
        Move = move;
        Round = round;
        Rng = rng;
        UserId = userId;
        LocalStorageId = localStorageId;
        CreatedDateUtc = createdDateUtc;
        LastUpdatedDateUtc = lastUpdatedDateUtc ?? createdDateUtc;

        UpdatePlayerStates(true);
    }

    public bool CanPlayMove(UserId? userId, StorageId? localStorageId)
    {
        return !HasWinner
               && PlayerBelongsToUser(ActivePlayer, userId, localStorageId);
    }

    public bool PlayerBelongsToUser(Player player, 
                                    UserId? userId, StorageId? localStorageId)
    {
        if (!player.IsHuman) return false;

        if (userId is not null && player.UserId is not null)
        {
            return userId.Id == player.UserId.Id;
        }

        if (localStorageId is not null && player.LocalStorageId is not null)
        {
            return localStorageId == player.LocalStorageId;
        }

        return userId is not null && userId.Id == UserId?.Id
               || LocalStorageId == localStorageId;
    }

    public bool CanPlaceAtom(GameBoard.Cell cell)
    {
        return cell.Player is null || cell.Player == ActivePlayer.Number;
    }

    public void PlaceAtom(int row, int column, Player player)
    {
        PlaceAtom(Board[row, column], player);
    }

    public void PlaceAtom(GameBoard.Cell cell, Player? player = null)
    {
        player ??= ActivePlayer;

        var cellPlayer = Players.FirstOrDefault(p => p.Number == cell.Player);

        cell.AddAtom(player);

        if (cellPlayer != null &&
            cellPlayer != player &&
            !Board.Cells.Any(c => c.Player == cellPlayer.Number))
        {
            cellPlayer.MarkDead();
        }
    }

    public void CheckForWinner() => UpdatePlayerStates(false);

    public void PostMoveUpdate()
    {
        SetNextActivePlayer();

        Move++;

        if (ActivePlayer == Players[0])
        {
            Round++;
        }
    }

    public Dictionary<Player, (PlayerScore, int)> GetScores()
    {
        List<PlayerScoreBuilder> playerScorers = [..
            Players.Select(player => new PlayerScoreBuilder(player))
        ];

        foreach (var cell in Board.Cells.Where(cell => cell.Player is not null))
        {
            playerScorers[cell.Player!.Value - 1].AddCell(cell.Atoms);
        }

        List<PlayerScore> playerScores = [..
            playerScorers.Select(ps => ps.Build())
        ];

        var playerScoresIndexed = playerScores
            .GroupBy(score => score.Score)
            .OrderByDescending(group => group.Key)
            .Select((g, i) => new { Item = g, Index = i});

        var playerScoresRanked = from item in playerScoresIndexed
                                 let rank = item.Index + 1
                                 from score in item.Item
                                 select new { Score = score, Rank = rank };

        return playerScoresRanked
            .ToDictionary(
                score => score.Score.Player,
                score => (score.Score, score.Rank));
    }

    public Player GetPlayer(Guid playerId)
    {
        return Players.First(p => p.Id == playerId);
    }

    public GameMenuOptions CreateOptionsForRematch(bool hasSound)
    {
        if (!HasWinner)
        {
            throw new InvalidOperationException("Game not finished");
        }

        var options = new GameMenuOptions
        {
            NumberOfPlayers = Players.Count,
            AtomShape = AtomShape,
            ColourScheme = ColourScheme,
            HasSound = hasSound
        };

        List<Player> players = [];

        var losingHumanPlayers = Players
            .Where(p => p.IsDead && p.IsHuman)
            .ToList();

        if (losingHumanPlayers.Count > 0)
        {
            players.AddRange(losingHumanPlayers.RandomSubset(1));
        }

        if (players.Count == 0)
        {
            var losingCPUPlayers = Players
                .Where(p => p.IsDead && !p.IsHuman)
                .ToList();

            if (losingCPUPlayers.Count > 0)
            {
                players.AddRange(losingCPUPlayers.RandomSubset(1));
            }
        }

        var otherPlayers = Players
            .Where(p => !players.Contains(p))
            .Shuffle();

        players.AddRange(otherPlayers);

        var playerOptions = players
            .Select((p, i) => new { Player = p, Number = i + 1 })
            .Select(p => new GameMenuOptions.Player { Number = p.Number, Type = p.Player.Type })
            .ToList();

        options.Players = playerOptions;

        return options;
    }

    internal void MarkCreated(DateTime createdDate)
    {
        LastUpdatedDateUtc = CreatedDateUtc = createdDate;
    }

    internal void MarkUpdated(DateTime lastUpdatedDate)
    {
        LastUpdatedDateUtc = lastUpdatedDate;
    }

    void UpdatePlayerStates(bool checkForDeadPlayers)
    {
        if (Round <= 1) return;

        var playerAtoms = (from player in Players
                           join cell in Board.Cells
                           on player.Number equals cell.Player into grp
                           select new
                           {
                               Player = player,
                               Atoms = grp.Sum(g => g.Atoms)
                           }).ToList();

        if (checkForDeadPlayers)
        {
            foreach (var item in playerAtoms)
            {
                if (item.Atoms == 0)
                {
                    item.Player.MarkDead();
                }
            }
        }

        if (playerAtoms.Count(pa => pa.Atoms > 0) == 1)
        {
            Winner = playerAtoms.First(pa => pa.Atoms > 0).Player;
        }
    }

    void SetNextActivePlayer()
    {
        var player = ActivePlayer;

        do
        {
            player = Players[player.Number % Players.Count];
        } while (player.IsDead);

        ActivePlayer = player;
    }

    public class Player
    {
        private readonly IPlayerStrategy? _strategy;

        public Player(Guid id,
                      int number,
                      PlayerType type,
                      UserId? userId = null,
                      string? name = null,
                      string? abbreviatedName = null,
                      IPlayerStrategy? strategy = null,
                      StorageId? localStorageId = null,
                      bool isDead = false)
        {
            if (type != PlayerType.Human && strategy is null)
            {
                throw new ArgumentNullException(nameof(strategy));
            }

            Id = id;
            Number = number;
            Type = type;
            UserId = userId;
            Name = name;
            AbbreviatedName = abbreviatedName;
            LocalStorageId = localStorageId;
            IsDead = isDead;

            _strategy = strategy;
        }

        public Guid Id { get; }
        public int Number { get; }
        public PlayerType Type { get; }
        public UserId? UserId { get; private set; }
        public string? Name { get; private set; }
        public string? AbbreviatedName { get; private set; }
        public StorageId? LocalStorageId { get; private set; }
        public bool IsDead { get; private set; }
        public bool IsHuman => Type == PlayerType.Human;
        public void MarkDead() => IsDead = true;

        public GameBoard.Cell? ChooseCell(Game game)
        {
            return _strategy?.Choose(game);
        }

        public void SetIdentity(UserId? userId,
                                string? name,
                                StorageId? localStorageId)
        {
            UserId = userId;
            Name = name;
            LocalStorageId = localStorageId;
        }

        public bool TryCreateInviteLink(string baseUrl, [NotNullWhen(true)] out InviteLink? inviteLink)
        {
            if (IsHuman && !IsDead && LocalStorageId is null)
            {
                inviteLink = new(Id.ToString("N"), baseUrl);

                return true;
            }

            inviteLink = null;

            return false;
        }

        public override string ToString()
        {
            var builder = new StringBuilder($"Player {Number}");

            if (IsHuman)
            {
                if (!string.IsNullOrEmpty(Name))
                {
                    builder.Append($" ({Name})");
                }
            }
            else
            {
                builder.Append($" ({Type.Description})");
            }

                return builder.ToString();
        }
    }

    public class GameBoard
    {
        static readonly (int, int)[] _offsets =
            [
                (0, -1),
                (0, 1),
                (-1, 0),
                (1, 0)
            ];

        public IReadOnlyList<Cell> Cells { get; }
        public int Rows { get; }
        public int Columns { get; }

        internal GameBoard(int rows,
                           int columns,
                           IEnumerable<CellState>? cells)
        {
            Rows = rows;
            Columns = columns;
            Cells = CreateCells(cells);
        }

        public Cell this[int row, int column] => Cells[GetCellIndex(row, column)];

        public IEnumerable<Cell> GetNeighbours(Cell cell) =>
            [.. from offset in _offsets
             let row = cell.Row + offset.Item1
             let column = cell.Column + offset.Item2
             let found = TryGetCell(row, column)
             where found != null
             select found];

        Cell? TryGetCell(int row, int column) =>
            CellExistsAt(row, column)
                ? this[row, column] : null;

        bool CellExistsAt(int row, int column) =>
            row > 0 && row <= Rows && column > 0 && column <= Columns;

        List<Cell> CreateCells(IEnumerable<CellState>? state)
        {
            int CalculateMaxAtoms(int row, int column) =>
                (row, column) switch
                {
                    (1, 1) => 1,
                    (1, var c) when c == Columns => 1,
                    (var r, 1) when r == Rows => 1,
                    (var r, var c) when r == Rows && c == Columns => 1,
                    (1, _) => 2,
                    (var r, _) when r == Rows => 2,
                    (_, 1) => 2,
                    (_, var c) when c == Columns => 2,
                    _ => 3
                };

            var cells = new List<Cell>(Rows * Columns);

            for (var row = 1; row <= Rows; row++)
            {
                for (var column = 1; column <= Columns; column++)
                {
                    var cellState = state
                        ?.FirstOrDefault(c =>
                            c.Row == row && c.Column == column);

                    var cell = new Cell(row, column, CalculateMaxAtoms(row, column), cellState?.Player, cellState?.Atoms);

                    cells.Add(cell);
                }
            }

            return cells;
        }

        int GetCellIndex(int row, int column) => (row - 1) * Columns + column - 1;

        public class Cell(int row, int column, int maxAtoms, int? player = null, int? atoms = null)
        {
            public int Row { get; } = row;
            public int Column { get; } = column;
            public int MaxAtoms { get; } = maxAtoms;
            public int? Player { get; private set; } = player;
            public int Atoms { get; private set; } = atoms ?? 0;
            public int FreeSpace => IsOverloaded ? 0 : MaxAtoms - Atoms;
            public bool IsOverloaded => Atoms > MaxAtoms;
            public bool IsFullyLoaded => Atoms == MaxAtoms;
            public ExplosionState Explosion { get; set; }

            internal void AddAtom(Player player)
            {
                Atoms++;
                Player = player.Number;
            }

            public void Explode()
            {
                Atoms -= MaxAtoms + 1;

                if (Atoms == 0) Player = null;
            }

            public override string ToString()
            {
                return $"Cell: {{ Row: {Row}, Column: {Column}, MaxAtoms: {MaxAtoms}, Player: {Player}, Atoms: {Atoms} }}";
            }
        }

        public record CellState(int Row, int Column, int Player, int Atoms);
    }

    private class PlayerScoreBuilder(Player player)
    {
        int _cells = 0;
        int _atoms = 0;
        int _score = 0;

        public void AddCell(int atoms)
        {
            _cells++;
            _atoms += atoms;
            _score += atoms switch
            {
                var x when x == 1 || x == 2 => x * 2,
                _ => 6
            };
        }

        public PlayerScore Build()
        {
            return new(player, _cells, _atoms, _score);
        }
    }

    public record PlayerScore(Player Player, int Cells, int Atoms, int Score);
}
