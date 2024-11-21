namespace Atoms.Core.Entities;

public class Game
{
    public IReadOnlyList<Player> Players { get; }
    public ColourScheme ColourScheme { get; }
    public AtomShape AtomShape { get; }
    public int Rows => Board.Rows;
    public int Columns => Board.Columns;
    public GameBoard Board { get; }
    public Player ActivePlayer { get; private set; }
    public Player? Winner { get; private set; }
    public bool IsInProgress { get; private set; } = true;
    public int Round { get; private set; }

    public Game(int rows,
                int columns,
                IReadOnlyList<Player> players,
                Player activePlayer,
                ColourScheme colourScheme,
                AtomShape atomShape,
                IEnumerable<GameBoard.CellState>? cells = null,
                int round = 1)
    {
        if (!players.Contains(activePlayer))
        {
            throw new ArgumentException(
                "Player not found", nameof(activePlayer));
        }

        ColourScheme = colourScheme;
        AtomShape = atomShape;
        Board = new GameBoard(rows, columns, cells, players);
        Players = players;
        ActivePlayer = activePlayer;
        Round = round;

        UpdateGameStatus();
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

    void UpdateGameStatus()
    {
        if (Round <= 1) return;

        var playerAtoms = from player in Players
                          join cell in Board.Cells
                          on player.Number equals cell.Player into grp
                          select new { Player = player, Atoms = grp.Sum(g => g.Atoms) };

        if (playerAtoms.Count(pa => pa.Atoms > 0) == 1)
        {
            Winner = playerAtoms.First(pa => pa.Atoms > 0).Player;
            IsInProgress = false;
        }
    }

    public void PostMoveUpdate()
    {
        var currentActivePlayer = ActivePlayer;

        SetNextActivePlayer();

        if (ActivePlayer == currentActivePlayer)
        {
            Winner = ActivePlayer;
            IsInProgress = false;
        }
        else if (ActivePlayer == Players[0])
        {
            Round++;
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

    public class Player(int number, PlayerType type)
    {
        public int Number { get; } = number;
        public PlayerType Type { get; } = type;
        public bool IsDead { get; private set; }

        public void MarkDead() => IsDead = true;
    }

    public class GameBoard
    {
        static readonly (int, int)[] _offsets =
            [
                (0, -1),
                (0, 1),
                (-1, 0),
                (1, 0),
            ];

        public IReadOnlyList<Cell> Cells { get; }
        public int Rows { get; }
        public int Columns { get; }

        internal GameBoard(int rows,
                           int columns,
                           IEnumerable<CellState>? cells,
                           IEnumerable<Player> players)
        {
            Rows = rows;
            Columns = columns;
            Cells = CreateCells(cells, players);
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

        List<Cell> CreateCells(IEnumerable<CellState>? state, IEnumerable<Player> players)
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
            public bool IsOverloaded => Atoms > MaxAtoms;
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
