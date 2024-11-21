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

        game.SetNextPlayerAsActive();

        return PlaceAtomResponse.Success;
    }

    async Task DoChainReaction(Game game, Stack<Cell> overloaded)
    {
        do
        {
            var cell = overloaded.Pop();

            await DoExplosion(game, cell);

            foreach (var neighbour in game.Board.GetNeighbours(cell))
            {
                await PlaceAtom(game, neighbour);

                if (neighbour.IsOverloaded)
                {
                    overloaded.Push(neighbour);
                }
            }
        } while (overloaded.Count > 0);
    }

    async Task PlaceAtom(Game game, Cell cell)
    {
        game.PlaceAtom(cell);

        await PublishNotification(game);
    }

    async Task DoExplosion(Game game, Cell cell)
    {
        cell.Explosion = ExplosionState.Before;
        cell.Explode();

        await PublishNotification(game);

        cell.Explosion = ExplosionState.After;

        await PublishNotification(game);

        cell.Explosion = ExplosionState.None;

        await PublishNotification(game);
    }

    async Task PublishNotification(Game game)
    {
        await mediator.Publish(new GameStateChanged(game));
    }
}
