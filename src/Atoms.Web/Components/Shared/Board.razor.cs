using Atoms.Web.CustomEvents;

namespace Atoms.Web.Components.Shared;

public class BoardComponent : Component2Base
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
