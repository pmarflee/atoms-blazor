namespace Atoms.Components.Email;

public class EmailConfirmationComponent : ComponentBase
{
    [Parameter]
    public string UserName { get; set; } = string.Empty;

    [Parameter]
    public string ConfirmationLink { get; set; } = string.Empty;

    [Parameter]
    public string FromAddress { get; set; } = string.Empty;

    [Parameter]
    public string LogoHtml { get; set; } = string.Empty;
}
