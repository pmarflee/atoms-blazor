using static Atoms.Core.Entities.Game.GameBoard;

namespace Atoms.UseCases.PlayerMove;

public class PlayerMoveRequestHandler(
    IMediator mediator,
    IDbContextFactory<ApplicationDbContext> dbContextFactory,
    IDateTimeService dateTimeService)
    : IRequestHandler<PlayerMoveRequest, PlayerMoveResponse>
{
    public async Task<PlayerMoveResponse> Handle(
        PlayerMoveRequest request,
        CancellationToken cancellationToken)
    {
        var game = request.Game;
        var cell = request.Cell;
        var requestPlayer = game.ActivePlayer;

        do
        {
            if (game.HasWinner)
            {
                return new PlayerMoveResponse(PlayerMoveResult.GameHasWinner);
            }

            var player = game.ActivePlayer;

            if (player.IsHuman)
            {
                if (cell is null || !game.CanPlaceAtom(cell))
                {
                    return new PlayerMoveResponse(PlayerMoveResult.InvalidMove);
                }

                if (await GameStateHasChanged(game, cancellationToken))
                {
                    return new PlayerMoveResponse(PlayerMoveResult.GameStateHasChanged);
                }
            }
            else
            {
                cell = player.ChooseCell(game);
            }

            if (cell is not null)
            {
                await PlaceAtom(game, cell, requestPlayer);

                var overloaded = new Stack<Cell>();

                if (cell.IsOverloaded)
                {
                    overloaded.Push(cell);

                    await DoChainReaction(game, overloaded, requestPlayer);
                }
            }

            if (!game.HasWinner)
            {
                game.PostMoveUpdate();
            }

            if (!request.Debug)
            {
                await NotifyPlayerMoved(game, player, requestPlayer);
            }    
        } while (!request.Debug && !game.HasWinner && !game.ActivePlayer.IsHuman);

        if (!request.Debug)
        {
            await SaveGame(game, cancellationToken);
            await NotifyGameSaved(game, requestPlayer);
        }

        return new PlayerMoveResponse(PlayerMoveResult.Ok);
    }

    async Task DoChainReaction(Game game, Stack<Cell> overloaded, Game.Player requestPlayer)
    {
        do
        {
            var cell = overloaded.Pop();

            if (!cell.IsOverloaded) continue;

            await DoExplosion(game, cell, requestPlayer);

            foreach (var neighbour in game.Board.GetNeighbours(cell))
            {
                await PlaceAtom(game, neighbour, requestPlayer);

                if (neighbour.Atoms == neighbour.MaxAtoms + 1)
                {
                    overloaded.Push(neighbour);
                }
            }

            game.CheckForWinner();

            if (game.HasWinner) break;
        } while (overloaded.Count > 0);
    }

    async Task PlaceAtom(Game game, Cell cell, Game.Player requestPlayer)
    {
        game.PlaceAtom(cell);

        await NotifyAtomPlaced(game, cell, requestPlayer);
    }

    async Task DoExplosion(Game game, Cell cell, Game.Player requestPlayer)
    {
        cell.Explosion = ExplosionState.Before;
        cell.Explode();

        await NotifyAtomExploded(game, cell, requestPlayer);

        cell.Explosion = ExplosionState.After;

        await NotifyAtomExploded(game, cell, requestPlayer);

        cell.Explosion = ExplosionState.None;

        await NotifyAtomExploded(game, cell, requestPlayer);
    }

    async Task NotifyAtomPlaced(Game game, Cell cell, Game.Player requestPlayer)
    {
        await mediator.Publish(
            new AtomPlaced(game.Id,
                           game.ActivePlayer.Id,
                           requestPlayer.Id,
                           cell.Row,
                           cell.Column));
    }

    async Task NotifyAtomExploded(Game game, Cell cell, Game.Player requestPlayer)
    {
        await mediator.Publish(
            new AtomExploded(game.Id,
                             game.ActivePlayer.Id,
                             requestPlayer.Id,
                             cell.Row,
                             cell.Column,
                             cell.Explosion));
    }

    async Task NotifyPlayerMoved(
        Game game, Game.Player player, Game.Player requestPlayer)
    {
        await mediator.Publish(new PlayerMoved(game.Id, player.Id, requestPlayer.Id));
    }

    async Task NotifyGameSaved(Game game, Game.Player player)
    {
        await mediator.Publish(new GameSaved(game.Id, player.Id));
    }

    static async Task<GameDTO> GetGameDTO(Game game,
                                          ApplicationDbContext dbContext,
                                          CancellationToken cancellationToken)
    {
        return await dbContext.GetGameById(game.Id, cancellationToken)
            ?? throw new Exception("Game not found");
    }

    async Task SaveGame(Game game, CancellationToken cancellationToken)
    {
        using var dbContext = await dbContextFactory.CreateDbContextAsync(
            cancellationToken);

        var gameDto = await GetGameDTO(game, dbContext, cancellationToken);

        gameDto.UpdateFromEntity(game, dateTimeService.UtcNow);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    async Task<bool> GameStateHasChanged(Game game,
                                         CancellationToken cancellationToken)
    {
        using var dbContext = await dbContextFactory.CreateDbContextAsync(
            cancellationToken);

        var gameDto = await GetGameDTO(game, dbContext, cancellationToken);

        return gameDto.LastUpdatedDateUtc != game.LastUpdatedDateUtc;
    }
}
