using Atoms.Core.Data;
using Microsoft.EntityFrameworkCore;

namespace Atoms.Core.Services;

public class GameCreationService(
    ILocalStorageUserService localStorageUserService,
    CreateGame gameFactory,
    IDbContextFactory<ApplicationDbContext> dbContextFactory
    ) : IGameCreationService
{
    public async Task<GameDTO> CreateGame(GameMenuOptions options,
                                          UserIdentity userIdentity,
                                          CancellationToken cancellationToken)
    {
        var localStorageId = await localStorageUserService.GetOrAddLocalStorageId(
            cancellationToken);

        var gameDto = gameFactory.Invoke(options, localStorageId, userIdentity);

        await SaveGame(gameDto, cancellationToken);

        return gameDto;
    }

    async Task SaveGame(GameDTO gameDto,
                        CancellationToken cancellationToken)
    {
        using var dbContext = await dbContextFactory.CreateDbContextAsync(
            cancellationToken);

        await dbContext.Games.AddAsync(gameDto, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
