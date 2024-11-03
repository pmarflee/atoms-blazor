namespace Atoms.Web.Components.Shared;

public class BoardComponent : ComponentBase
{
    [Parameter]
    public Game Game { get; set; } = default!;

    protected override void OnParametersSet()
    {
    }
}
