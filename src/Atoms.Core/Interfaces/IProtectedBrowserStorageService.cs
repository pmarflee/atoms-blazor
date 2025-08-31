namespace Atoms.Core.Interfaces;

public interface IProtectedBrowserStorageService
{
    ValueTask<TValue?> GetAsync<TValue>(string key);
    ValueTask SetAsync(string key, object value);
}
