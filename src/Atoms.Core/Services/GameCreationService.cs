using Atoms.Core.Data;
using Microsoft.EntityFrameworkCore;

namespace Atoms.Core.Services;

public class GameCreationService(
    IVisitorService visitorService,
    CreateGame gameFactory,
    IDbContextFactory<ApplicationDbContext> dbContextFactory
    ) : IGameCreationService
{
    public async Task<GameDTO> CreateGame(GameMenuOptions options,
                                          VisitorId visitorId,
                                          UserIdentity userIdentity,
                                          CancellationToken cancellationToken)
    {
        await visitorService.SaveVisitorId(visitorId, cancellationToken);

        var gameDto = gameFactory.Invoke(options, visitorId, userIdentity);

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
