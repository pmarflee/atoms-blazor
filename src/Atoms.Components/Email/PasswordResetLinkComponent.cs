namespace Atoms.Components.Email;

public class PasswordResetLinkComponent : ComponentBase
{
    [Parameter]
    public string UserName { get; set; } = string.Empty;

    [Parameter]
    public string ResetLink { get; set; } = string.Empty;

    [Parameter]
    public string FromAddress { get; set; } = string.Empty;

    [Parameter]
    public string LogoUrl { get; set; } = string.Empty;
}
