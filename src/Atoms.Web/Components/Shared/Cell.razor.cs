namespace Atoms.Web.Components.Shared;

public partial class CellComponent : Component2Base
{
    protected string Id = default!;
    protected string Style = default!;

    [Parameter]
    public Game.GameBoard.Cell Data { get; set; } = default!;

    [Parameter]
    public string PlayerClassName { get; set; } = default!;

    [Parameter]
    public bool CanHighlight { get; set;  }

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
        await OnCellClicked.InvokeAsync(
            new CellClickEventArgs(Data.Position));
    }

    protected string CellClasses =>
        string.Join(" ",
            [ 
                "cell",
                CanHighlight && Data.Highlighted ? "highlighted" : "filled"
            ]);

    protected string AtomClasses(int atom)
    {
        var pos = atom switch
        {
            var x when Data.Atoms < 5 => Data.Atoms - x + 1, 
            var x when x < 5 => 5 - x,
            _ => 1
        };

        return string.Join(" ", 
            [
                "atom",
                $"atom{atom}",
                PlayerClassName,
                $"pos{pos}"
            ]);
    }

    protected string ExplosionClasses()
    {
        return string.Join(" ",
            [
                "explosion",
                Data.Explosion switch
                {
                    ExplosionState.Before => "start",
                    ExplosionState.After => "end",
                    _ => ""
                }
            ]);
    }
}
