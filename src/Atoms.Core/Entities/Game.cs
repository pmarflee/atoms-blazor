namespace Atoms.Core.Entities;

public class Game
{
    public IReadOnlyList<Player> Players { get; }
    public ColourScheme ColourScheme { get; }
    public AtomShape AtomShape { get; }
    public int Rows => Board.Rows;
    public int Columns => Board.Columns;
    public GameBoard Board { get; }

    internal Game(int rows,
                  int columns,
                  IReadOnlyList<Player> players,
                  ColourScheme colourScheme,
                  AtomShape atomShape)
    {
        Players = players;
        ColourScheme = colourScheme;
        AtomShape = atomShape;
        Board = new GameBoard(rows, columns);
        Players[0].IsActive = true;
    }

    public class Player
    {
        public int Number { get; }
        public PlayerType Type { get; }
        public bool IsActive { get; set; }

        internal Player(int number, PlayerType type)
        {
            Number = number;
            Type = type;
        }
    }

    public class GameBoard
    {
        readonly Cell[,] _cells;

        public int Rows => _cells.GetLength(0);
        public int Columns => _cells.GetLength(1);

        internal GameBoard(int rows, int columns)
        {
            _cells = CreateCells(rows, columns);
        }

        public IEnumerable<Cell> Cells =>
            from row in Enumerable.Range(0, Rows)
            from column in Enumerable.Range(0, Columns)
            select this[row, column];

        public Cell this[int row, int column] => _cells[row, column];

        private static Cell[,] CreateCells(int rows, int columns)
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
        }
    }
}
