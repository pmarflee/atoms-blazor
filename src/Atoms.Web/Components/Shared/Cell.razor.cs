using static Atoms.Core.Entities.Game;

namespace Atoms.Web.Components.Shared;

public partial class CellComponent : ComponentBase
{
    protected string Id = default!;
    protected string Style = default!;

    [Parameter]
    public GameBoard.Cell Data { get; set; } = default!;

    protected override void OnParametersSet()
    {
        Id = $"cell_{Data.Row}_{Data.Column}";
        Style = $"grid-area: {Data.Row} / {Data.Column}";
        
        base.OnParametersSet();
    }
}
