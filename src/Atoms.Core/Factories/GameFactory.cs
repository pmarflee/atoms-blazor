using Atoms.Core.DTOs;
using Atoms.Core.Entities;
using Atoms.Core.Interfaces;

namespace Atoms.Core.Factories;

public class GameFactory : IGameFactory
{
    const int Rows = 6;
    const int Columns = 10;

    public Game Create(GameMenu menuDto)
    {
        var players = menuDto.Players
            .Take(menuDto.NumberOfPlayers)
            .Select(p => new Game.Player(p.Number, p.Type))
            .ToList();

        return new Game(Rows,
                        Columns,
                        players,
                        menuDto.ColourScheme,
                        menuDto.AtomShape);
    }
}
