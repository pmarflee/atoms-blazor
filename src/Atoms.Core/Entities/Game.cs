using Atoms.Core.Interfaces;

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
                IEnumerable<GameBoard.CellState>? cells = null,
                int move = 1,
                int round = 1)
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

        CheckForWinner();
    }

    public bool CanPlaceAtom(GameBoard.Cell cell)
    {
        return cell.Player is null || cell.Player == ActivePlayer.Number;
    }

    public void PlaceAtom(GameBoard.Cell cell)
    {
        var cellPlayer = Players.FirstOrDefault(p => p.Number == cell.Player);

        cell.AddAtom(ActivePlayer);

        if (cellPlayer != null &&
            cellPlayer != ActivePlayer &&
            !Board.Cells.Any(c => c.Player == cellPlayer.Number))
        {
            cellPlayer.MarkDead();
        }
    }

    public void CheckForWinner()
    {
        if (Round <= 1) return;

        var playerAtoms = from player in Players
                          join cell in Board.Cells
                          on player.Number equals cell.Player into grp
                          select new { Player = player, Atoms = grp.Sum(g => g.Atoms) };

        if (playerAtoms.Count(pa => pa.Atoms > 0) == 1)
        {
            Winner = playerAtoms.First(pa => pa.Atoms > 0).Player;
        }
    }

    public void PostMoveUpdate()
    {
        SetNextActivePlayer();

        Move++;

        if (ActivePlayer == Players[0])
        {
            Round++;
        }
    }

    public (int, int) GetScore(Player player)
    {
        var cells = 0;
        var atoms = 0;

        foreach (var cell in Board.Cells.Where(cell => cell.Player == player.Number))
        {
            cells++;
            atoms += cell.Atoms;
        }

        return (cells, atoms);
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

        public Player(int number, PlayerType type, IPlayerStrategy? strategy = null)
        {
            if (type != PlayerType.Human && strategy is null)
            {
                throw new ArgumentNullException(nameof(strategy));
            }

            Number = number;
            Type = type;

            _strategy = strategy;
        }

        public int Number { get; }
        public int Id => Number - 1;
        public PlayerType Type { get; }
        public bool IsDead { get; private set; }
        public bool IsHuman => Type == PlayerType.Human;
        public void MarkDead() => IsDead = true;

        public GameBoard.Cell? ChooseCell(Game game)
        {
            return _strategy?.Choose(game);
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
            (from offset in _offsets
             let row = cell.Row + offset.Item1
             let column = cell.Column + offset.Item2
             let found = TryGetCell(row, column)
             where found != null
             select found).ToList();

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
}
