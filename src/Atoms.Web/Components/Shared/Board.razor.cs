using Atoms.Web.CustomEvents;
using static Atoms.Core.Entities.Game;

namespace Atoms.Web.Components.Shared;

public class BoardComponent : ComponentBase
{
    [Parameter]
    public Game Game { get; set; } = default!;

    protected override void OnParametersSet()
    {
    }

    protected void CellClicked(CellClickEventArgs eventArgs)
    {
        if (Game.CanPlaceAtom(eventArgs.Cell))
        {
            Game.PlaceAtom(eventArgs.Cell);
        }
    }
}
