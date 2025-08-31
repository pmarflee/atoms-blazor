
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace Atoms.Infrastructure.Services;

public class ProtectedBrowserStorageService(
    ProtectedLocalStorage protectedLocalStorage) 
    : IProtectedBrowserStorageService
{
    public async ValueTask<TValue?> GetAsync<TValue>(string key)
    {
        var result = await protectedLocalStorage.GetAsync<TValue>(key);

        return result.Success ? result.Value : default;
    }

    public async ValueTask SetAsync(string key, object value)
    {
        await protectedLocalStorage.SetAsync(key, value);
    }
}
