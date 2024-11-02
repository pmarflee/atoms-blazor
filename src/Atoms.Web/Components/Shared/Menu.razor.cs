namespace Atoms.Web.Components.Shared;

public partial class MenuComponent : ComponentBase
{
    [Parameter]
    public EventCallback<Game> OnCreateGame { get; set; }

    [Inject]
    public IGameFactory GameFactory { get; set; } = default!;

    protected GameMenu GameMenu { get; set; } = default!;

    protected override void OnInitialized()
    {
        GameMenu = new(GameMenu.MinPlayers, GameMenu.MaxPlayers);
    }

    protected async Task SubmitAsync()
    {
        var game = GameFactory.Create(GameMenu);

        await OnCreateGame.InvokeAsync(game);
    }
}
