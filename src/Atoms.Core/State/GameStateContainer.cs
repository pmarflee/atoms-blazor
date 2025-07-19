namespace Atoms.Core.State;

public class GameStateContainer
{
    private Game? _game;
    private GameState _state;

    public Game? Game
    {
        get => _game;
    }

    public GameState State
    {
        get => _state;
    }

    public async Task SetGame(Game game, bool isReload)
    {
        _game = game;
        _state = GameState.Game;

        await NotifyGameSet(isReload);
        await NotifyStateChanged();
    }

    public async Task GameUpdated()
    {
        await NotifyStateChanged();
    }

    public async Task GameReloadRequired()
    {
        await NotifyGameReloadRequired();
    }

    public async Task PlayerMoved(int playerNumber, string? playerName)
    {
        await NotifyPlayerMoved(playerNumber, playerName);
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
    public event Func<int, string?, Task> OnPlayerMoved = default!;

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

    async Task NotifyPlayerMoved(int playerNumber, string? playerName) => 
        await OnPlayerMoved.Invoke(playerNumber, playerName);
}
