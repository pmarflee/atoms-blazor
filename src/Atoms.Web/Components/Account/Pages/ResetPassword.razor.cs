namespace Atoms.Web.Components.Account.Pages;

public partial class ResetPasswordComponent : ComponentBase
{
    [Inject]
    public IdentityRedirectManager RedirectManager { get; set; } = default!;

    [Inject]
    public UserManager<ApplicationUser> UserManager { get; set; } = default!;

    private IEnumerable<IdentityError>? identityErrors;

    [SupplyParameterFromForm]
    protected InputModel Input { get; set; } = default!;

    [SupplyParameterFromQuery]
    protected string? Code { get; set; }

    protected string? Message => identityErrors is null ? null : $"Error: {string.Join(", ", identityErrors.Select(error => error.Description))}";

    protected override void OnInitialized()
    {
        if (Code is null)
        {
            RedirectManager.RedirectTo("Account/InvalidPasswordReset");
        }

        Input ??= new();    
        Input.Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(Code));
    }

    protected async Task OnValidSubmit()
    {
        var user = await UserManager.FindByEmailAsync(Input.Email);
        if (user is null)
        {
            // Don't reveal that the user does not exist
            RedirectManager.RedirectTo("Account/ResetPasswordConfirmation");
        }

        var result = await UserManager.ResetPasswordAsync(user, Input.Code, Input.Password);
        if (result.Succeeded)
        {
            RedirectManager.RedirectTo("Account/ResetPasswordConfirmation");
        }

        identityErrors = result.Errors;
    }

    protected sealed class InputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = "";

        [Required]
        public string Code { get; set; } = "";
    }
}