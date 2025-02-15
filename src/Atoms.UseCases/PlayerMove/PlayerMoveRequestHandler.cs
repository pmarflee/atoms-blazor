using static Atoms.Core.Entities.Game.GameBoard;

namespace Atoms.UseCases.PlayerMove;

public class PlayerMoveRequestHandler(
    IMediator mediator,
    IDbContextFactory<ApplicationDbContext> dbContextFactory)
    : IRequestHandler<PlayerMoveRequest, PlayerMoveResponse>
{
    public async Task<PlayerMoveResponse> Handle(
        PlayerMoveRequest request,
        CancellationToken cancellationToken)
    {
        var game = request.Game;
        var cell = request.Cell;

        if (game.ActivePlayer.IsHuman)
        {
            if (cell is null || !game.CanPlaceAtom(cell))
            {
                return PlayerMoveResponse.Failure;
            }
        }
        else
        {
            cell = game.ActivePlayer.ChooseCell(game);
        }

        if (cell is not null)
        {
            await PlaceAtom(game, cell);

            var overloaded = new Stack<Cell>();

            if (cell.IsOverloaded)
            {
                overloaded.Push(cell);

                await DoChainReaction(game, overloaded);
            }
        }

        if (!game.HasWinner)
        {
            game.PostMoveUpdate();
        }

        await SaveGame(game, cancellationToken);

        return PlayerMoveResponse.Success;
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

    async Task SaveGame(Game game,
                        CancellationToken cancellationToken)
    {
        using var dbContext = await dbContextFactory.CreateDbContextAsync(
            cancellationToken);

        var gameDto = await dbContext.GetGameById(game.Id,
                                                  cancellationToken) 
            ?? throw new Exception("Game not found");

        gameDto.UpdateFromEntity(game);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
