namespace Atoms.UseCases.CreateNewGame;

public class CreateNewGameRequestHandler(
    Func<GameMenuOptions, Game> gameFactory,
    IDbContextFactory<ApplicationDbContext> dbContextFactory)
    : IRequestHandler<CreateNewGameRequest, CreateNewGameResponse>
{
    public async Task<CreateNewGameResponse> Handle(
        CreateNewGameRequest request,
        CancellationToken cancellationToken)
    {
        var game = gameFactory.Invoke(request.Options);

        await SaveGame(game, request.StorageId, cancellationToken);

        return new CreateNewGameResponse(game);
    }

    async Task SaveGame(Game game,
                        StorageId storageId,
                        CancellationToken cancellationToken)
    {
        using var dbContext = await dbContextFactory.CreateDbContextAsync(
            cancellationToken);

        var gameDto = GameDTO.FromEntity(game, storageId);

        await dbContext.Games.AddAsync(gameDto, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
