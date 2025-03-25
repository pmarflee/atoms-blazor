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

    public async ValueTask<ColourScheme> GetColourScheme()
    {
        var returnValue = ColourScheme.Original;

        try
        {
            var result = await protectedLocalStore.GetAsync<ColourScheme>(
                Constants.StorageKeys.ColourScheme);

            if (result.Success)
            {
                returnValue = result.Value;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unable to retrieve colour scheme selection from local storage");
        }

        return returnValue;
    }

    public async ValueTask SetColourScheme(ColourScheme colourScheme)
    {
        await protectedLocalStore.SetAsync(
            Constants.StorageKeys.ColourScheme,
            colourScheme);
    }

    public async ValueTask<AtomShape> GetAtomShape()
    {
        var returnValue = AtomShape.Round;

        try
        {
            var result = await protectedLocalStore.GetAsync<AtomShape>(
                Constants.StorageKeys.AtomShape);

            if (result.Success)
            {
                returnValue = result.Value;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unable to retrieve atom shape selection from local storage");
        }

        return returnValue;
    }

    public async ValueTask SetAtomShape(AtomShape colourScheme)
    {
        await protectedLocalStore.SetAsync(
            Constants.StorageKeys.AtomShape,
            colourScheme);
    }

    public async ValueTask<bool> GetSound()
    {
        var returnValue = true;

        try
        {
            var result = await protectedLocalStore.GetAsync<bool>(
                Constants.StorageKeys.Sound);

            if (result.Success)
            {
                returnValue = result.Value;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unable to retrieve sound selection from local storage");
        }

        return returnValue;
    }

    public async ValueTask SetSound(bool hasSound)
    {
        await protectedLocalStore.SetAsync(
            Constants.StorageKeys.Sound,
            hasSound);
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
