using Atoms.Core.Data.Identity;
using Atoms.Core.Identity;

namespace Atoms.UseCases.SetUserName;

public class SetUserNameRequestHandler(
    IDbContextFactory<ApplicationDbContext> applicationDbContextFactory,
    IDbContextFactory<ApplicationIdentityDbContext> identityDbContextFactory,
    IVisitorService visitorService)
    : IRequestHandler<SetUserNameRequest>
{
    public async Task Handle(SetUserNameRequest request,
                             CancellationToken cancellationToken)
    {
        var applicationDbContext = await applicationDbContextFactory.CreateDbContextAsync(cancellationToken);
        var userIdentity = request.UserIdentity;
        var userName = userIdentity.Name!;
        var userId = userIdentity.Id;
        var game = request.Game;
        var visitorId = request.VisitorId;

        if (game is not null)
        {
            var gameDto = await applicationDbContext.GetGameById(game.Id, cancellationToken);
            var playerDto = gameDto!.Players
                .OrderBy(p => p.Number)
                .First(p => p.PlayerTypeId == PlayerType.Human);

            if (string.IsNullOrEmpty(playerDto.AbbreviatedName))
            {
                playerDto.AbbreviatedName = request.UserIdentity.GetAbbreviatedName();
            }
        }

        await visitorService.AddOrUpdate(visitorId, userName, cancellationToken);

        if (userId is not null)
        {
            var identityDbContext = await identityDbContextFactory.CreateDbContextAsync(cancellationToken);
            var user = await identityDbContext.FindAsync<ApplicationUser>(userId.Id);

            if (user is not null)
            {
                user.Name = userName;

                await identityDbContext.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
