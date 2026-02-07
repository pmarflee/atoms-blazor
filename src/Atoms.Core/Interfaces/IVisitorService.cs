using Atoms.Core.Data;

namespace Atoms.Core.Interfaces;

public interface IVisitorService
{
    Task SaveVisitorId(VisitorId visitorId, CancellationToken? cancellationToken = null);
    Task SaveVisitorId(
        VisitorId visitorId,
        ApplicationDbContext dbContext, 
        CancellationToken? cancellationToken = null);
}
