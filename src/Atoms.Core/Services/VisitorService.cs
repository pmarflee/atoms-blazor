using Microsoft.EntityFrameworkCore;
using Atoms.Core.Data;

namespace Atoms.Core.Services;

public class VisitorService(
    IDbContextFactory<ApplicationDbContext> dbContextFactory)
    : IVisitorService
{
    public async Task AddOrUpdate(
        VisitorId visitorId,
        CancellationToken? cancellationToken = null)
    {
        await AddOrUpdate(visitorId, null, cancellationToken);
    }

    public async Task AddOrUpdate(
        VisitorId visitorId,
        string? name,
        CancellationToken? cancellationToken = null)
    {
        cancellationToken ??= CancellationToken.None;

        var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken.Value);

        await dbContext.AddOrUpdateVisitor(
            visitorId, name, cancellationToken.Value);
    }
}
