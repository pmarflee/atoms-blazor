﻿using static Atoms.Core.Entities.Game;

namespace Atoms.UnitTests;

internal static class ObjectMother
{
    public const int Rows = 6;
    public const int Columns = 10;

    public static Game Game(List<Player>? players = null,
                            int? active = 1,
                            List<GameBoard.CellState>? cells = null,
                            int move = 1,
                            int round = 1)
    {
        players ??=
        [
            new (1, PlayerType.Human),
            new (2, PlayerType.Human),
        ];

        var activePlayer = players.First(p => p.Number == active);

        return new Game(Guid.Empty, Rows, Columns, players, activePlayer,
                        ColourScheme.Original, AtomShape.Round,
                        cells, move, round);
    }
}
