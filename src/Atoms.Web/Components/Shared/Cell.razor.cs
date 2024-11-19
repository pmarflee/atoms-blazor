using Atoms.Web.CustomEvents;

namespace Atoms.Web.Components.Shared;

public partial class CellComponent : Component2Base
{
    protected string Id = default!;
    protected string Style = default!;

    [Parameter]
    public Game.GameBoard.Cell Data { get; set; } = default!;

    [Parameter]
    public EventCallback<CellClickEventArgs> OnCellClicked { get; set; }

    protected override void OnParametersSet()
    {
        Id = $"cell_{Data.Row}_{Data.Column}";
        Style = $"grid-area: {Data.Row} / {Data.Column}";
        
        base.OnParametersSet();
    }

    protected async Task CellClick()
    {
        await OnCellClicked.InvokeAsync(new CellClickEventArgs(Data));
    }

    protected string AtomClasses(int atom)
    {
        return string.Join(" ", 
            [
                "atom",
                $"atom{atom}",
                Data.Player!.ClassName,
                $"pos{Data.Atoms - atom + 1}"
            ]);
    }
}
