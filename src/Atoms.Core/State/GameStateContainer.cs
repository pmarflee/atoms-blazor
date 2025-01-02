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

    public async Task SetGame(Game game)
    {
        _game = game;
        _state = GameState.Game;

        await NotifyStateChanged();
    }

    public async Task GameUpdated()
    {
        await NotifyStateChanged();
    }

    public async Task SetMenu()
    {
        _game = null;
        _state = GameState.Menu;

        await NotifyStateChanged();
    }

    public event Func<Task> OnChange = default!;
    private async Task NotifyStateChanged() => await OnChange.Invoke();
}
