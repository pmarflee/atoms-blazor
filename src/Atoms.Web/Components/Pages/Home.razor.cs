using Atoms.Core.Enums;

namespace Atoms.Web.Components.Pages;

public partial class HomePageComponent : ComponentBase
{
    protected GameState State { get; set; }
    protected GameMenu? Menu { get; set; }

    protected override void OnInitialized()
    {
        State = GameState.Menu;
        Menu = new(GameMenu.MinPlayers, GameMenu.MaxPlayers);
    }

    protected void Submit()
    {

    }
}