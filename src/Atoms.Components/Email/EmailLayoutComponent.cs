namespace Atoms.Components.Email;

public class EmailLayoutComponent : ComponentBase
{
    [Parameter]
    public string Title { get; set; } = string.Empty;

    [Parameter]
    public string FromAddress { get; set; } = string.Empty;

    [Parameter]
    public string LogoUrl { get; set; } = string.Empty;

    [Parameter]
    public RenderFragment? ChildContent { get; set; }
}
