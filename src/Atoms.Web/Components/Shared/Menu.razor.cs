using Atoms.Core.Enums;

namespace Atoms.Web.Components.Shared;

public partial class MenuComponent : ComponentBase
{
    [Parameter]
    public EventCallback<Game> OnCreateGame { get; set; }

    [Inject]
    public IGameFactory GameFactory { get; set; } = default!;

    protected MenuState State { get; set; }
    protected GameMenu GameMenu { get; set; } = default!;

    protected override void OnInitialized()
    {
        GameMenu = new(GameMenu.MinPlayers, GameMenu.MaxPlayers);
        State = MenuState.Menu;
    }

    protected async Task SubmitAsync()
    {
        var game = GameFactory.Create(GameMenu);

        await OnCreateGame.InvokeAsync(game);
    }

    protected void ShowAbout()
    {
        State = MenuState.About;
    }

    protected void HideAbout()
    {
        State = MenuState.Menu;
    }
}
