namespace Atoms.Web.Services;

public class BrowserStorageService(ProtectedLocalStorage protectedLocalStore)
{
    public async Task<StorageId> GetOrAddStorageId()
    {
        return await GetStorageId() ?? await CreateStorageId();
    }

    public async Task<StorageId?> GetStorageId()
    {
        var localStorageIdResult =
            await protectedLocalStore.GetAsync<Guid>(
                Constants.StorageKeys.LocalStorageId);

        return localStorageIdResult.Success 
            ? new(localStorageIdResult.Value) 
            : null;
    }

    async Task<StorageId> CreateStorageId()
    {
        var id = Guid.CreateVersion7();

        await protectedLocalStore.SetAsync(
            Constants.StorageKeys.LocalStorageId,
            id);

        return new(id);
    }
}
