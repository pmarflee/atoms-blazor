using Atoms.Core.Entities;
using static Atoms.Core.Entities.Game.GameBoard;

namespace Atoms.UseCases.PlaceAtom;

public class PlaceAtomRequest(Game game, Cell cell) : IRequest<PlaceAtomResponse>
{
    public Game Game { get; } = game;
    public Cell Cell { get; } = cell;
}
