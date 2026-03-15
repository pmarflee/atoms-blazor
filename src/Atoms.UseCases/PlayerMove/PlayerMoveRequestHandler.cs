using Atoms.UseCases.PlayerMove.Rebus;
using Microsoft.Extensions.Logging;

namespace Atoms.UseCases.PlayerMove;

public class PlayerMoveRequestHandler(
    IDbContextFactory<ApplicationDbContext> dbContextFactory,
    IBus bus,
    ILogger<PlayerMoveRequestHandler> logger)
    : IRequestHandler<PlayerMoveRequest, PlayerMoveResponse>
{
    public async Task<PlayerMoveResponse> Handle(
        PlayerMoveRequest request,
        CancellationToken cancellationToken)
    {
        var game = request.Game;
        var cell = request.Position is not null
            ? game.Board[request.Position.Row, request.Position.Column]
            : null;

        if (game.HasWinner)
        {
            return new PlayerMoveResponse(PlayerMoveResult.GameHasWinner);
        }

        if (game.ActivePlayer.IsHuman)
        {
            if (cell is null || !game.CanPlaceAtom(cell))
            {
                return new PlayerMoveResponse(PlayerMoveResult.InvalidMove);
            }

            var gameDto = await GetGameDTO(game, cancellationToken);

            if (gameDto is null)
            {
                return new PlayerMoveResponse(PlayerMoveResult.GameDoesNotExist);
            }

            var (gameStateHasChanged, allowRetry) = GameStateHasChanged(
                game, gameDto);

            if (gameStateHasChanged)
            {
                return new PlayerMoveResponse(PlayerMoveResult.GameStateHasChanged, allowRetry);
            }
        }

        if (logger.IsEnabled(LogLevel.Debug))
        {
            logger.LogDebug("Posting message to bus. GameId='{GameId}'.", game.Id);
        }

        await bus.Send(
            new PlayerMoveMessage(
                game.Id, cell?.Row, cell?.Column,
                game.LastUpdatedDateUtc));

        return new PlayerMoveResponse(PlayerMoveResult.Ok);
    }

    async Task<GameDTO?> GetGameDTO(Game game, CancellationToken cancellationToken)
    {
        using var dbContext = await dbContextFactory.CreateDbContextAsync(
            cancellationToken);

        return await dbContext.GetGameById(game.Id, cancellationToken);
    }

    static (bool HasChanged, bool AllowRetry) GameStateHasChanged(
        Game game, GameDTO gameDto)
    {
        var changed = gameDto.LastUpdatedDateUtc != game.LastUpdatedDateUtc;
        var allowRetry = changed && game.Move == gameDto.Move;

        return new(changed, allowRetry);
    }
}
