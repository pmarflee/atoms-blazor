namespace Atoms.Web.Services;

public class BrowserStorageService(ProtectedLocalStorage protectedLocalStore)
{
    public async Task<StorageId> GetOrAddStorageId()
    {
        var localStorageIdResult =
            await protectedLocalStore.GetAsync<Guid>(
                Constants.StorageKeys.LocalStorageId);

        Guid id;

        if (localStorageIdResult.Success)
        {
            id = localStorageIdResult.Value;
        }
        else
        {
            id = Guid.CreateVersion7();

            await protectedLocalStore.SetAsync(
                Constants.StorageKeys.LocalStorageId,
                id);
        }

        return new(id);
    }
}
