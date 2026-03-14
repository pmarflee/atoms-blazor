using Atoms.Core.Data.Identity;
using Atoms.Core.Identity;

namespace Atoms.UseCases.DeleteUserData;

public class DeleteUserDataRequestHandler(
    IDbContextFactory<ApplicationDbContext> applicationDbContextFactory,
    IDbContextFactory<ApplicationIdentityDbContext> identityDbContextFactory)
    : IRequestHandler<DeleteUserDataRequest>
{
    public async Task Handle(DeleteUserDataRequest request,
                             CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var userId = request.UserId?.Id;
        var visitorId = request.VisitorId.Value;

        var applicationDbContext = await applicationDbContextFactory.CreateDbContextAsync(cancellationToken) ?? throw new InvalidOperationException("Failed to create application database context");
        var identityDbContext = await identityDbContextFactory.CreateDbContextAsync(cancellationToken) ?? throw new InvalidOperationException("Failed to create identity database context");

        // Get all games where this user is the owner
        var ownedGamesByUser = userId != null
            ? applicationDbContext.Games
                .Where(g => g.UserId == userId)
                .ToList()
            : [];

        // Get all games where this visitor is the owner
        var ownedGamesByVisitor = applicationDbContext.Games
            .Where(g => g.VisitorId == visitorId)
            .ToList();

        // Get all games where this user is a player (but not the owner)
        var gamesAsPlayerUser = userId != null
            ? applicationDbContext.Games
                .Where(g => !ownedGamesByUser.Contains(g) && g.Players.Any(p => p.UserId == userId))
                .ToList()
            : [];

        // Get all games where this visitor is a player (but not the owner)
        var gamesAsPlayerVisitor = applicationDbContext.Games
            .Where(g => !ownedGamesByVisitor.Contains(g) && g.Players.Any(p => p.VisitorId == visitorId))
            .ToList();

        // Delete owned games entirely
        foreach (var game in ownedGamesByUser)
        {
            // Delete players first
            applicationDbContext.Players.RemoveRange(game.Players);
            applicationDbContext.Games.Remove(game);
        }

        foreach (var game in ownedGamesByVisitor)
        {
            // Delete players first
            applicationDbContext.Players.RemoveRange(game.Players);
            applicationDbContext.Games.Remove(game);
        }

        // Remove user reference from games where they are a player but not the owner
        foreach (var game in gamesAsPlayerUser)
        {
            foreach (var player in game.Players.Where(p => p.UserId == userId))
            {
                player.UserId = null;
            }
        }

        // Remove visitor reference from games where they are a player but not the owner
        foreach (var game in gamesAsPlayerVisitor)
        {
            foreach (var player in game.Players.Where(p => p.VisitorId == visitorId))
            {
                player.VisitorId = null;
            }
        }

        await applicationDbContext.SaveChangesAsync(cancellationToken);

        // Delete the visitor
        var visitor = await applicationDbContext.Visitors.FindAsync([visitorId], cancellationToken: cancellationToken);
        if (visitor != null)
        {
            applicationDbContext.Visitors.Remove(visitor);
            await applicationDbContext.SaveChangesAsync(cancellationToken);
        }

        // Delete the user from identity if UserId is provided
        if (userId != null)
        {
            var user = await identityDbContext.FindAsync<ApplicationUser>([userId], cancellationToken: cancellationToken);

            if (user != null)
            {
                identityDbContext.Users.Remove(user);
                await identityDbContext.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
