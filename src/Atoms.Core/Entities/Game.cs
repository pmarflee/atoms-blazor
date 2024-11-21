


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
                    cell.Player!.Value,
                    cell.Atoms)
                )
                .ToList(),
            ActivePlayer.Number,
            ColourScheme,
            AtomShape);

    public bool CanPlaceAtom(GameBoard.Cell cell)
    {
        return cell.Player is null || cell.Player == ActivePlayer.Number;
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

        public IReadOnlyList<Cell> Cells { get; }
        public int Rows { get; }
        public int Columns { get; }

        internal GameBoard(int rows,
                           int columns,
                           IEnumerable<State.Cell>? cellState,
                           IEnumerable<Player> players)
        {
            Rows = rows;
            Columns = columns;

            Cells = CreateCells();

            if (cellState != null)
            {
                AddCellState(cellState, players);
            }
        }

        public Cell this[int row, int column] => Cells[GetCellIndex(row, column)];

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

        List<Cell> CreateCells()
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
                    var maxAtoms = CalculateMaxAtoms(row, column);

                    cells.Add(new(row, column, maxAtoms));
                }
            }

            return cells;
        }

        int GetCellIndex(int row, int column) => (row - 1) * Columns + column - 1;

        public class Cell(int row, int column, int maxAtoms)
        {
            public int Row { get; } = row;
            public int Column { get; } = column;
            public int MaxAtoms { get; } = maxAtoms;
            public int? Player { get; private set; }
            public int Atoms { get; private set; }
            public bool IsOverloaded => Atoms > MaxAtoms;

            internal void AddAtom(Player player)
            {
                Atoms++;
                Player = player.Number;
            }

            internal void Explode()
            {
                Atoms -= MaxAtoms + 1;

                if (Atoms == 0) Player = null;
            }

            internal void LoadState(Player player, int atoms)
            {
                ArgumentOutOfRangeException.ThrowIfGreaterThan(atoms, MaxAtoms);

                Player = player.Number;
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
