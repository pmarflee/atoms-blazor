using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Atoms.Infrastructure.Data;

internal class AuditingSaveChangesInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var dbContext = eventData.Context;

        if (dbContext is not null)
        {
            foreach (var entry in dbContext.ChangeTracker.Entries().Where(e => e.State == EntityState.Added || e.State == EntityState.Modified))
            {
                if (entry.Entity is IAuditable auditable)
                {
                    if (entry.State == EntityState.Added)
                    {
                        auditable.LastUpdatedDateUtc = auditable.CreatedDateUtc = DateTime.UtcNow;
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        auditable.LastUpdatedDateUtc = DateTime.UtcNow;
                    }
                }
            }
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
