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

        // Load all potentially affected games in a single query (includes players)
        var games = applicationDbContext.GetGamesForUserOrVisitor(userId, visitorId);

        // Single pass: delete games owned by the user or visitor, and
        // null out user/visitor references for games where they are players but not the owner.
        foreach (var game in games)
        {
            var isOwnedByUser = userId != null && game.UserId == userId;
            var isOwnedByVisitor = game.VisitorId == visitorId;

            if (isOwnedByUser || isOwnedByVisitor)
            {
                // Delete players first then the game
                applicationDbContext.Players.RemoveRange(game.Players);
                applicationDbContext.Games.Remove(game);
                continue;
            }

            // Not owned by the user/visitor - clear player fields according to rules
            foreach (var player in game.Players.Where(p => (userId != null && p.UserId == userId) || p.VisitorId == visitorId))
            {
                // Always clear visitorId
                player.VisitorId = null;

                // Only clear userId if userId is specified in the request
                if (userId != null && player.UserId == userId)
                {
                    player.UserId = null;
                    // Always clear abbreviated name if userId is specified
                    player.AbbreviatedName = null;
                }
                else if (userId == null)
                {
                    // Only clear abbreviated name if player is not linked to a user
                    if (player.UserId == null)
                    {
                        player.AbbreviatedName = null;
                    }
                }
            }
        }

        await applicationDbContext.SaveChangesAsync(cancellationToken);

        // Delete the visitor
        var visitor = await applicationDbContext.Visitors.FindAsync(new object?[] { visitorId, cancellationToken }, cancellationToken: cancellationToken);
        if (visitor != null)
        {
            applicationDbContext.Visitors.Remove(visitor);
            await applicationDbContext.SaveChangesAsync(cancellationToken);
        }

        // Delete the user from identity if UserId is provided
        if (userId != null)
        {
            var user = await identityDbContext.FindAsync<ApplicationUser>(userId, cancellationToken);

            if (user != null)
            {
                identityDbContext.Users.Remove(user);
                await identityDbContext.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
