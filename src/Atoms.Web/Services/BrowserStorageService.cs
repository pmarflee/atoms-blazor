
using System.Text.Json;

namespace Atoms.Web.Services;

public class BrowserStorageService(
    ProtectedLocalStorage protectedLocalStore, 
    ILogger<BrowserStorageService> logger)
    : IBrowserStorageService
{
    public async Task<StorageId> GetOrAddStorageId()
    {
        return await GetStorageId() ?? await CreateStorageId();
    }

    public async Task<StorageId?> GetStorageId()
    {
        StorageId? returnValue = null;

        try
        {
            var localStorageIdResult =
                await protectedLocalStore.GetAsync<Guid>(
                    Constants.StorageKeys.LocalStorageId);

            if (localStorageIdResult.Success)
            {
                returnValue = new(localStorageIdResult.Value);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unable to retrieve local storage id from local storage");
        }

        return returnValue;
    }

    public async ValueTask<string?> GetUserName()
    {
        string? returnValue = null;

        try
        {
            var result = await protectedLocalStore.GetAsync<string>(
                Constants.StorageKeys.UserName);

            if (result.Success)
            {
                returnValue = result.Value;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unable to retrieve user name from local storage");
        }

        return returnValue;
    }

    public async ValueTask SetUserName(string userName)
    {
        await protectedLocalStore.SetAsync(
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
        var result = await protectedLocalStore.GetAsync<string>(
            Constants.StorageKeys.GameMenuOptions);

        if (!result.Success) return null;

        try
        {
            return JsonSerializer.Deserialize<GameMenuOptions>(result.Value!);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unable to retrieve game menu options from local storage");

            return null;
        }
    }

    public async ValueTask SetGameMenuOptions(GameMenuOptions options)
    {
        await protectedLocalStore.SetAsync(
            Constants.StorageKeys.GameMenuOptions,
            JsonSerializer.Serialize(options));
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
