namespace Atoms.Core.Interfaces;

public interface IBrowserStorageService
{
    Task<StorageId> GetOrAddStorageId();
    Task<StorageId?> GetStorageId();
    ValueTask<string?> GetUserName();
    ValueTask SetUserName(string userName);
    ValueTask<bool> GetSound();
    ValueTask<GameMenuOptions?> GetGameMenuOptions();
    ValueTask SetGameMenuOptions(GameMenuOptions options);
}