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

        ValueTask<VisitorDTO> GetVisitorById(VisitorId visitorId) =>
            applicationDbContext.FindAsync<VisitorDTO>(visitorId.Value)!;

        async ValueTask<GetGameResponse> Found()
        {
            return GetGameResponse.Found(
                await gameDto.ToEntity(
                    rngFactory,
                    playerStrategyFactory,
                    GetUserById,
                    GetVisitorById));
        }

        bool GameMatchesUserId() =>
            request.UserId is not null &&
            (gameDto.UserId == request.UserId ||
            gameDto.Players.Any(p => p.UserId == request.UserId));

        bool GameMatchesVisitorId() =>
            gameDto.VisitorId == request.VisitorId.Value ||
            gameDto.Players.Any(p => p.VisitorId == request.VisitorId.Value);

        return GameMatchesUserId() || GameMatchesVisitorId()
            ? await Found()
            : GetGameResponse.NotFound;
    }
}
