using Atoms.UseCases.PlayerMove.Rebus;

namespace Atoms.UseCases.PlayerMove;

public class PlayerMoveRequestHandler(
    IDbContextFactory<ApplicationDbContext> dbContextFactory, IBus bus)
    : IRequestHandler<PlayerMoveRequest, PlayerMoveResponse>
{
    public async Task<PlayerMoveResponse> Handle(
        PlayerMoveRequest request,
        CancellationToken cancellationToken)
    {
        var game = request.Game;
        var cell = request.Cell;

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

            if (await GameStateHasChanged(game, cancellationToken))
            {
                return new PlayerMoveResponse(PlayerMoveResult.GameStateHasChanged);
            }
        }

        await bus.Send(
            new PlayerMoveMessage(
                game.Id, cell?.Row, cell?.Column, game.LastUpdatedDateUtc));

        return new PlayerMoveResponse(PlayerMoveResult.Ok);
    }

    static async Task<GameDTO> GetGameDTO(Game game,
                                          ApplicationDbContext dbContext,
                                          CancellationToken cancellationToken)
    {
        return await dbContext.GetGameById(game.Id, cancellationToken)
            ?? throw new Exception("Game not found");
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
