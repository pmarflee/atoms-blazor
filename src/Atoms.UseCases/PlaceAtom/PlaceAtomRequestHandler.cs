using Atoms.Core.Entities;
using static Atoms.Core.Entities.Game.GameBoard;

namespace Atoms.UseCases.PlaceAtom;

public class PlaceAtomRequestHandler(IMediator mediator) 
    : IRequestHandler<PlaceAtomRequest, PlaceAtomResponse>
{
    public Task<PlaceAtomResponse> Handle(PlaceAtomRequest request,
                       CancellationToken cancellationToken)
    {
        var game = request.Game;
        var cell = request.Cell;

        if (!game.CanPlaceAtom(cell))
        {
            return Task.FromResult(PlaceAtomResponse.Failure);
        }

        game.PlaceAtom(cell);

        var overloaded = new Stack<Cell>();

        if (cell.IsOverloaded)
        {
            overloaded.Push(cell);

            DoChainReaction(game, overloaded);
        }

        game.SetNextPlayerAsActive();

        return Task.FromResult(PlaceAtomResponse.Success);
    }

    void DoChainReaction(Game game, Stack<Cell> overloaded)
    {
        do
        {
            var cell = overloaded.Pop();

            cell.Explode();

            foreach (var neighbour in game.Board.GetNeighbours(cell))
            {
                game.PlaceAtom(neighbour);

                if (neighbour.IsOverloaded)
                {
                    overloaded.Push(neighbour);
                }
            }
        } while (overloaded.Count > 0);
    }
}
