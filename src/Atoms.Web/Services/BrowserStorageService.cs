namespace Atoms.Web.Services;

public class BrowserStorageService(ProtectedLocalStorage protectedLocalStore) 
    : IBrowserStorageService
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

    public async ValueTask<string?> GetUserName()
    {
        var result = await protectedLocalStore.GetAsync<string>(
            Constants.StorageKeys.UserName);

        return result.Success ? result.Value : null;
    }

    public async ValueTask SetUserName(string userName)
    {
        await protectedLocalStore.SetAsync(
            Constants.StorageKeys.UserName,
            userName);
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
