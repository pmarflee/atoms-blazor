namespace Atoms.Core.Interfaces;

public interface IBrowserStorageService
{
    ValueTask<StorageId> GetOrAddStorageId();
    ValueTask<StorageId?> GetStorageId();
    ValueTask<bool> GetSound();
    ValueTask<GameMenuOptions?> GetGameMenuOptions();
    ValueTask SetGameMenuOptions(GameMenuOptions options);
}