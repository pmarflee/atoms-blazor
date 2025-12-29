namespace Atoms.Web.Components.Account.Pages;

public partial class ForgotPasswordComponent : ComponentBase
{
    [Inject]
    public UserManager<ApplicationUser> UserManager { get; set; } = default!;

    [Inject]
    public IEmailSender<ApplicationUser> EmailSender { get; set; } = default!;

    [Inject]
    public NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    public IdentityRedirectManager RedirectManager { get; set; } = default!;

    [SupplyParameterFromForm]
    protected InputModel Input { get; set; } = default!;

    protected async Task OnValidSubmit()
    {
        var user = await UserManager.FindByEmailAsync(Input.Email);

        if (user is null || !(await UserManager.IsEmailConfirmedAsync(user)))
        {
            // Don't reveal that the user does not exist or is not confirmed
            RedirectManager.RedirectTo("Account/ForgotPasswordConfirmation");
        }

        // For more information on how to enable account confirmation and password reset please
        // visit https://go.microsoft.com/fwlink/?LinkID=532713
        var code = await UserManager.GeneratePasswordResetTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        var callbackUrl = NavigationManager.GetUriWithQueryParameters(
            NavigationManager.ToAbsoluteUri("Account/ResetPassword").AbsoluteUri,
            new Dictionary<string, object?> { ["code"] = code });

        await EmailSender.SendPasswordResetLinkAsync(user, Input.Email, HtmlEncoder.Default.Encode(callbackUrl));

        RedirectManager.RedirectTo("Account/ForgotPasswordConfirmation");
    }

    protected sealed class InputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";
    }
}