namespace Atoms.Core.Interfaces;

public interface ILocalStorageUserService
{
    Task<StorageId> GetOrAddLocalStorageId(CancellationToken cancellationToken);
}
