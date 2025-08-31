using Atoms.Core.Delegates;
using Blazored.LocalStorage;

namespace Atoms.Infrastructure.Services;

public class BrowserStorageService(
    ILocalStorageService localStorageService,
    IProtectedBrowserStorageService protectedBrowserStorageService,
    CreateLocalStorageId createLocalStorageId)
    : IBrowserStorageService
{
    public async ValueTask<StorageId> GetOrAddStorageId()
    {
        return await GetStorageId() ?? await CreateStorageId();
    }

    public async ValueTask<StorageId?> GetStorageId()
    {
        var id = await protectedBrowserStorageService.GetAsync<Guid?>(
                Constants.StorageKeys.LocalStorageId);

        return id.HasValue ? new(id.Value) : null;
    }

    public async ValueTask<string?> GetUserName()
    {
        return await localStorageService.GetItemAsync<string?>(
            Constants.StorageKeys.UserName);
    }

    public async ValueTask SetUserName(string userName)
    {
        await localStorageService.SetItemAsync(
            Constants.StorageKeys.UserName,
            userName);
    }

    public async ValueTask<bool> GetSound()
    {
        var options = await GetGameMenuOptions();

        return options?.HasSound ?? true;
    }

    public async ValueTask<GameMenuOptions?> GetGameMenuOptions()
    {
        return await localStorageService.GetItemAsync<GameMenuOptions?>(
            Constants.StorageKeys.GameMenuOptions);
    }

    public async ValueTask SetGameMenuOptions(GameMenuOptions options)
    {
        await localStorageService.SetItemAsync(
            Constants.StorageKeys.GameMenuOptions,
            options);
    }

    async Task<StorageId> CreateStorageId()
    {
        var id = createLocalStorageId.Invoke();

        await protectedBrowserStorageService.SetAsync(
            Constants.StorageKeys.LocalStorageId,
            id);

        return new(id);
    }
}
