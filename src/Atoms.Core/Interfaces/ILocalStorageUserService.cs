using Atoms.Core.Data;

namespace Atoms.Core.Interfaces;

public interface ILocalStorageUserService
{
    Task<StorageId> GetOrAddLocalStorageId(CancellationToken? cancellationToken = null);
    Task<StorageId> GetOrAddLocalStorageId(
        ApplicationDbContext dbContext, CancellationToken? cancellationToken = null);
}
