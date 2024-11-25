using static Atoms.Core.Entities.Game.GameBoard;

namespace Atoms.UseCases.PlaceAtom;

public class PlaceAtomRequestHandler(IMediator mediator)
    : IRequestHandler<PlaceAtomRequest, PlaceAtomResponse>
{
    public async Task<PlaceAtomResponse> Handle(PlaceAtomRequest request,
                                                CancellationToken cancellationToken)
    {
        var game = request.Game;
        var cell = request.Cell;

        if (!game.CanPlaceAtom(cell)) return PlaceAtomResponse.Failure;

        await PlaceAtom(game, cell);

        var overloaded = new Stack<Cell>();

        if (cell.IsOverloaded)
        {
            overloaded.Push(cell);

            await DoChainReaction(game, overloaded);
        }

        if (!game.HasWinner)
        {
            game.PostMoveUpdate();
        }

        return PlaceAtomResponse.Success;
    }

    async Task DoChainReaction(Game game, Stack<Cell> overloaded)
    {
        do
        {
            var cell = overloaded.Pop();

            if (!cell.IsOverloaded) continue;

            await DoExplosion(game, cell);

            foreach (var neighbour in game.Board.GetNeighbours(cell))
            {
                await PlaceAtom(game, neighbour);

                if (neighbour.Atoms == neighbour.MaxAtoms + 1)
                {
                    overloaded.Push(neighbour);
                }
            }

            game.CheckForWinner();

            if (game.HasWinner) break;
        } while (overloaded.Count > 0);
    }

    async Task PlaceAtom(Game game, Cell cell)
    {
        game.PlaceAtom(cell);

        await NotifyAtomPlaced(game);
    }

    async Task DoExplosion(Game game, Cell cell)
    {
        cell.Explosion = ExplosionState.Before;
        cell.Explode();

        await NotifyAtomExploded(game);

        cell.Explosion = ExplosionState.After;

        await NotifyAtomExploded(game);

        cell.Explosion = ExplosionState.None;

        await NotifyAtomExploded(game);
    }

    async Task NotifyAtomPlaced(Game game)
    {
        await mediator.Publish(new AtomPlaced(game));
    }

    async Task NotifyAtomExploded(Game game)
    {
        await mediator.Publish(new AtomExploded(game));
    }
}
