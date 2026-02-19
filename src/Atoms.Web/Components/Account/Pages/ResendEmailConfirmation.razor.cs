namespace Atoms.Web.Components.Account.Pages;

public partial class ResendEmailConfirmationComponent : ComponentBase
{
    [Inject]
    public UserManager<ApplicationUser> UserManager { get; set; } = default!;

    [Inject]
    public IEmailSender<ApplicationUser> EmailSender { get; set; } = default!;

    [Inject]
    public NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    public IdentityRedirectManager RedirectManager { get; set; } = default!;

    protected string? Message;

    [SupplyParameterFromForm]
    protected InputModel Input { get; set; } = default!;

    protected override void OnInitialized()
    {
        Input ??= new();    
    }

    protected async Task OnValidSubmit()
    {
        var user = await UserManager.FindByEmailAsync(Input.Email!);
        if (user is null)
        {
            Message = "Verification email sent. Please check your email.";
            return;
        }

        var userId = await UserManager.GetUserIdAsync(user);
        var code = await UserManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        var callbackUrl = NavigationManager.GetUriWithQueryParameters(
            NavigationManager.ToAbsoluteUri("Account/ConfirmEmail").AbsoluteUri,
            new Dictionary<string, object?> { ["userId"] = userId, ["code"] = code });
        await EmailSender.SendConfirmationLinkAsync(user, Input.Email, HtmlEncoder.Default.Encode(callbackUrl));

        Message = "Verification email sent. Please check your email.";
    }

    protected sealed class InputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";
    }
}