namespace Atoms.Components.Email;

public class PasswordResetCodeComponent : ComponentBase
{
    [Parameter]
    public string UserName { get; set; } = string.Empty;

    [Parameter]
    public string ResetCode { get; set; } = string.Empty;

    [Parameter]
    public string FromAddress { get; set; } = string.Empty;

    [Parameter]
    public string LogoHtml { get; set; } = string.Empty;
}
