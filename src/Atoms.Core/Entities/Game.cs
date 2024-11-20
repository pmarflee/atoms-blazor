


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

    Stack<GameBoard.Cell> Overloaded { get; } = new();

    internal Game(int rows,
                  int columns,
                  IEnumerable<Player> players,
                  Player activePlayer,
                  ColourScheme colourScheme,
                  AtomShape atomShape,
                  IEnumerable<State.Cell>? cellState = null)
    {
        Players = new List<Player>(players);
        ColourScheme = colourScheme;
        AtomShape = atomShape;
        Board = new GameBoard(rows, columns, cellState, Players);

        if (!Players.Contains(activePlayer))
        {
            throw new ArgumentException(
                "Player not found", nameof(activePlayer));
        }

        ActivePlayer = activePlayer;
    }

    public static Game Load(State state)
    {
        var players = state.Players
            .Select(p => new Player(p.Number, p.Type))
            .ToList();

        var activePlayer = players.First(p => p.Number == state.ActivePlayer);

        return new(state.Rows,
                   state.Columns,
                   players,
                   activePlayer,
                   state.ColourScheme,
                   state.AtomShape,
                   state.Cells);
    }

    public State Save() =>
        new(Rows,
            Columns,
            Players
                .Select(player => new State.Player(
                    player.Number,
                    player.Type)
                )
                .ToList(),
            Board.Cells
                .Where(cell => cell.Player is not null)
                .Select(cell => new State.Cell(
                    cell.Row,
                    cell.Column,
                    cell.Player!.Number,
                    cell.Atoms)
                )
                .ToList(),
            ActivePlayer.Number,
            ColourScheme,
            AtomShape);

    public bool CanPlaceAtom(GameBoard.Cell cell)
    {
        return cell.Player is null || cell.Player == ActivePlayer;
    }

    public void PlaceAtom(GameBoard.Cell cell)
    {
        cell.AddAtom(ActivePlayer);

        if (cell.IsOverloaded)
        {
            Overloaded.Push(cell);
            DoChainReaction();
        }

        SetActivePlayer();
    }

    void SetActivePlayer()
    {
        ActivePlayer = Players[ActivePlayer.Number % Players.Count];
    }

    void DoChainReaction()
    {
        do
        {
            var cell = Overloaded.Pop();

            cell.Explode();

            foreach (var neighbour in Board.GetNeighbours(cell))
            {
                neighbour.AddAtom(ActivePlayer);

                if (neighbour.IsOverloaded)
                {
                    Overloaded.Push(neighbour);
                }
            }
        } while (Overloaded.Count > 0);
    }

    public class Player
    {
        public int Number { get; }
        public PlayerType Type { get; }

        internal Player(int number, PlayerType type)
        {
            Number = number;
            Type = type;
        }
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

        readonly Cell[,] _cells;

        public int Rows => _cells.GetLength(0);
        public int Columns => _cells.GetLength(1);

        internal GameBoard(int rows,
                           int columns,
                           IEnumerable<State.Cell>? cellState,
                           IEnumerable<Player> players)
        {
            _cells = CreateCells(rows, columns);

            if (cellState != null)
            {
                AddCellState(cellState, players);
            }
        }

        public IEnumerable<Cell> Cells =>
            from row in Enumerable.Range(1, Rows)
            from column in Enumerable.Range(1, Columns)
            select this[row, column];

        public Cell this[int row, int column] => _cells[row - 1, column - 1];

        public IEnumerable<Cell> GetNeighbours(Cell cell) =>
            (from offset in _offsets
            let row = cell.Row + offset.Item1
            let column = cell.Column + offset.Item2
            let found = TryGetCell(row, column)
            where found != null
            select found).ToList();

        void AddCellState(IEnumerable<State.Cell> cellState, IEnumerable<Player> players)
        {
            foreach (var item in cellState)
            {
                var cell = this[item.Row, item.Column];
                var player = players.First(p => p.Number == item.Player);

                cell.LoadState(player, item.Atoms);
            }
        }

        Cell? TryGetCell(int row, int column) =>
            CellExistsAt(row, column)
                ? this[row, column] : null;

        bool CellExistsAt(int row, int column) =>
            row > 0 && row <= Rows && column > 0 && column <= Columns;

        static Cell[,] CreateCells(int rows, int columns)
        {
            int CalculateMaxAtoms(int row, int column) =>
                (row, column) switch
                {
                    (1, 1) => 1,
                    (1, var c) when c == columns => 1,
                    (var r, 1) when r == rows => 1,
                    (var r, var c) when r == rows && c == columns => 1,
                    (1, _) => 2,
                    (var r, _) when r == rows => 2,
                    _ => 3
                };

            var cells = new Cell[rows, columns];

            for (var row = 0; row < rows; row++)
            {
                for (var column = 0; column < columns; column++)
                {
                    var cellRow = row + 1;
                    var cellColumn = column + 1;
                    var maxAtoms = CalculateMaxAtoms(cellRow, cellColumn);

                    cells[row, column] = new(cellRow, cellColumn, maxAtoms);
                }
            }

            return cells;
        }

        public class Cell(int row, int column, int maxAtoms)
        {
            public int Row { get; } = row;
            public int Column { get; } = column;
            public int MaxAtoms { get; } = maxAtoms;
            public Player? Player { get; private set; }
            public int Atoms { get; private set; }
            public bool IsOverloaded => Atoms > MaxAtoms;

            internal void AddAtom(Player player)
            {
                Atoms++;
                Player = player;
            }

            internal void Explode()
            {
                Atoms -= MaxAtoms + 1;

                if (Atoms == 0) Player = null;
            }

            internal void LoadState(Player player, int atoms)
            {
                ArgumentOutOfRangeException.ThrowIfGreaterThan(atoms, MaxAtoms);

                Player = player;
                Atoms = atoms;
            }
        }
    }

    public record State(int Rows,
                        int Columns,
                        List<State.Player> Players,
                        List<State.Cell> Cells,
                        int ActivePlayer,
                        ColourScheme ColourScheme = ColourScheme.Original,
                        AtomShape AtomShape = AtomShape.Round)
    {
        public record Player(int Number, PlayerType Type);

        public record Cell(int Row, int Column, int Player, int Atoms);
    }
}
