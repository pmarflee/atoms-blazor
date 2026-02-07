namespace Atoms.Core.Interfaces;

public interface IBrowserStorageService
{
    ValueTask<bool> GetSound();
    ValueTask<GameMenuOptions?> GetGameMenuOptions();
    ValueTask SetGameMenuOptions(GameMenuOptions options);
}