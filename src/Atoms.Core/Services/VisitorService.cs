using Microsoft.EntityFrameworkCore;
using Atoms.Core.Data;

namespace Atoms.Core.Services;

public class VisitorService(
    IDbContextFactory<ApplicationDbContext> dbContextFactory)
    : IVisitorService
{
    public async Task SaveVisitorId(
        VisitorId visitorId,
        CancellationToken? cancellationToken = null)
    {
        cancellationToken ??= CancellationToken.None;

        var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken.Value);

        await SaveVisitorId(visitorId, dbContext, cancellationToken);
    }

    public async Task SaveVisitorId(
        VisitorId visitorId,
        ApplicationDbContext dbContext,
        CancellationToken? cancellationToken = null)
    {
        cancellationToken ??= CancellationToken.None;

        await dbContext.AddVisitorId(visitorId, cancellationToken.Value);
    }
}
