namespace Atoms.UseCases.GetGame;

public class GetGameRequestHandler(
    CreateRng rngFactory,
    CreatePlayerStrategy playerStrategyFactory,
    IDbContextFactory<ApplicationDbContext> dbContextFactory)
    : IRequestHandler<GetGameRequest, GetGameResponse>
{
    public async Task<GetGameResponse> Handle(
        GetGameRequest request,
        CancellationToken cancellationToken)
    {
        using var dbContext = await dbContextFactory.CreateDbContextAsync(
            cancellationToken);

        var gameDto = await dbContext.GetGameById(request.GameId,
                                                  cancellationToken);

        if (gameDto is null) return GetGameResponse.NotFound;

        GetGameResponse Found() =>
            GetGameResponse.Found(
                gameDto.ToEntity(rngFactory, playerStrategyFactory));

        if (request.UserId is not null)
        {
            return gameDto.Players.Any(p => p.UserId == request.UserId)
                ? Found()
                : GetGameResponse.NotFound;
        }

        return gameDto.LocalStorageId == request.StorageId.Value
            ? Found()
            : GetGameResponse.NotFound;
    }
}
