
namespace Atoms.Core.State;

public class GameStateContainer
{
    Game? _game;
    GameState _state;
    StorageId _localStorageId = default!;

    public Game? Game => _game;

    public GameState State => _state;

    public StorageId LocalStorageId => _localStorageId;

    public async Task SetGame(Game game, bool isReload)
    {
        _game = game;
        _state = GameState.Game;

        await NotifyGameSet(isReload);
        await NotifyStateChanged();
    }

    public void SetLocalStorageId(StorageId localStorageId)
    {
        _localStorageId = localStorageId;
    }

    public async Task GameUpdated()
    {
        await NotifyStateChanged();
    }

    public async Task GameReloadRequired()
    {
        await NotifyGameReloadRequired();
    }

    public async Task SetMenu()
    {
        _game = null;
        _state = GameState.Menu;

        await NotifyStateChanged();
    }

    public event Func<Task> OnChange = default!;
    public event Func<bool, Task>? OnGameSet;
    public event Func<Task> OnGameReloadRequired = default!;

    async Task NotifyStateChanged() => await OnChange.Invoke();

    async Task NotifyGameSet(bool isReload)
    {
        if (OnGameSet is not null)
        {
            await OnGameSet.Invoke(isReload);
        }
    }

    async Task NotifyGameReloadRequired() => 
        await OnGameReloadRequired.Invoke();
}
