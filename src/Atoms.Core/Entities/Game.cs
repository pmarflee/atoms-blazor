namespace Atoms.Core.Entities;

public class Game
{
    public IReadOnlyList<Player> Players { get; }
    public ColourScheme ColourScheme { get; }
    public AtomShape AtomShape { get; }
    Grid GameGrid { get; }

    internal Game(int rows,
                  int columns,
                  IReadOnlyList<Player> players,
                  ColourScheme colourScheme,
                  AtomShape atomShape)
    {
        Players = players;
        ColourScheme = colourScheme;
        AtomShape = atomShape;
        GameGrid = new Grid(rows, columns);
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

    public class Grid
    {
        readonly Cell[,] _cells;

        internal Grid(int rows, int columns)
        {
            _cells = CreateCells(rows, columns);
        }

        internal Cell this[int row, int column] => _cells[row, column];

        private static Cell[,] CreateCells(int rows, int columns)
        {
            var cells = new Cell[rows, columns];

            for (var row = 0; row < rows; row++)
            {
                for (var column = 0; column < columns; column++)
                {
                    cells[row, column] = new();
                }
            }

            return cells;
        }

        public class Cell
        {
            public Player? Player { get; private set; }
            public int Atoms { get; private set; }
        }
    }
}
