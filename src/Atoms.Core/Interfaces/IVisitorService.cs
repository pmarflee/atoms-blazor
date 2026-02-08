namespace Atoms.Core.Interfaces;

public interface IVisitorService
{
    Task AddOrUpdate(
        VisitorId visitorId,
        CancellationToken? cancellationToken = null);

    Task AddOrUpdate(
        VisitorId visitorId,
        string? name,
        CancellationToken? cancellationToken = null);
}
