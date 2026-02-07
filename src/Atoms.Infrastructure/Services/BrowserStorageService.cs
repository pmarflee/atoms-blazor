using Blazored.LocalStorage;

namespace Atoms.Infrastructure.Services;

public class BrowserStorageService(ILocalStorageService localStorageService)
    : IBrowserStorageService
{
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
}
