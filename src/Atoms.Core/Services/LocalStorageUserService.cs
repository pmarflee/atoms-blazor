using Microsoft.EntityFrameworkCore;
using Atoms.Core.Data;

namespace Atoms.Core.Services;

public class LocalStorageUserService(
    IBrowserStorageService browserStorageService,
    IDbContextFactory<ApplicationDbContext> dbContextFactory)
    : ILocalStorageUserService
{
    public async Task<StorageId> GetOrAddLocalStorageId(
        CancellationToken? cancellationToken = null)
    {
        cancellationToken ??= CancellationToken.None;

        var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken.Value);

        return await GetOrAddLocalStorageId(dbContext, cancellationToken);
    }

    public async Task<StorageId> GetOrAddLocalStorageId(
        ApplicationDbContext dbContext, CancellationToken? cancellationToken = null)
    {
        cancellationToken ??= CancellationToken.None;

        var storageId = await browserStorageService.GetOrAddStorageId();

        await dbContext.AddOrUpdateLocalStorageId(storageId, cancellationToken.Value);

        return storageId;
    }
}
