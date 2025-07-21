namespace Atoms.UseCases.CreateNewGame;

public class CreateNewGameRequestHandler(
    CreateGame gameFactory,
    IDbContextFactory<ApplicationDbContext> dbContextFactory)
    : IRequestHandler<CreateNewGameRequest, CreateNewGameResponse>
{
    public async Task<CreateNewGameResponse> Handle(
        CreateNewGameRequest request,
        CancellationToken cancellationToken)
    {
        var game = gameFactory.Invoke(
            request.Options, request.LocalStorageId, request.UserIdentity);

        await SaveGame(game, cancellationToken);

        return new CreateNewGameResponse(game);
    }

    async Task SaveGame(Game game,
                        CancellationToken cancellationToken)
    {
        using var dbContext = await dbContextFactory.CreateDbContextAsync(
            cancellationToken);

        var gameDto = GameDTO.FromEntity(game);

        await dbContext.Games.AddAsync(gameDto, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
