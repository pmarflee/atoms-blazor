using Microsoft.EntityFrameworkCore;
using Atoms.Core.Data;

namespace Atoms.Core.Services;

public class LocalStorageUserService(
    IBrowserStorageService browserStorageService,
    IDbContextFactory<ApplicationDbContext> dbContextFactory)
    : ILocalStorageUserService
{
    public async Task<StorageId> GetOrAddLocalStorageId(
        CancellationToken cancellationToken)
    {
        var storageId = await browserStorageService.GetOrAddStorageId();
        var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        var localStorageUserExists = await dbContext.LocalStorageUsers
            .AnyAsync(u => u.Id == storageId.Value, cancellationToken);

        if (!localStorageUserExists)
        {
            var localStorageUser = new LocalStorageUserDTO { Id = storageId.Value };

            dbContext.LocalStorageUsers.Add(localStorageUser);

            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return storageId;
    }
}
