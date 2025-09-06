using Atoms.Core.Data.Identity;
using Atoms.Core.Identity;

namespace Atoms.UseCases.GetGame;

public class GetGameRequestHandler(
    CreateRng rngFactory,
    CreatePlayerStrategy playerStrategyFactory,
    IDbContextFactory<ApplicationDbContext> applicationDbContextFactory,
    IDbContextFactory<ApplicationIdentityDbContext> applicationIdentityDbContextFactory)
    : IRequestHandler<GetGameRequest, GetGameResponse>
{
    public async Task<GetGameResponse> Handle(
        GetGameRequest request,
        CancellationToken cancellationToken)
    {
        using var applicationDbContext = 
            await applicationDbContextFactory.CreateDbContextAsync(
                cancellationToken);

        using var applicationIdentityDbContext =
            await applicationIdentityDbContextFactory.CreateDbContextAsync(
                cancellationToken);

        var gameDto = await applicationDbContext.GetGameById(
            request.GameId, cancellationToken);

        if (gameDto is null) return GetGameResponse.NotFound;

        ValueTask<ApplicationUser> GetUserById(UserId userId) =>
            applicationIdentityDbContext.FindAsync<ApplicationUser>(userId.Id)!;

        ValueTask<LocalStorageUserDTO> GetLocalStorageUserById(StorageId localStorageId) =>
            applicationDbContext.FindAsync<LocalStorageUserDTO>(localStorageId.Value)!;

        async ValueTask<GetGameResponse> Found()
        {
            return GetGameResponse.Found(
                await gameDto.ToEntity(
                    rngFactory,
                    playerStrategyFactory,
                    GetUserById,
                    GetLocalStorageUserById));
        }

        bool GameMatchesUserId() =>
            request.UserId is not null &&
            (gameDto.UserId == request.UserId ||
            gameDto.Players.Any(p => p.UserId == request.UserId));

        bool GameMatchesLocalStorageId() =>
            gameDto.LocalStorageUserId == request.StorageId.Value ||
            gameDto.Players.Any(p => p.LocalStorageUserId == request.StorageId.Value);

        return GameMatchesUserId() || GameMatchesLocalStorageId()
            ? await Found()
            : GetGameResponse.NotFound;
    }
}
