﻿namespace Atoms.Web.Components.Account.Pages;

public class RegisterConfirmationComponent : ComponentBase
{
    [Inject]
    public UserManager<ApplicationUser> UserManager { get; set; } = default!;

    [Inject]
    public IEmailSender<ApplicationUser> EmailSender { get; set; } = default!;

    [Inject]
    public NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    public IdentityRedirectManager RedirectManager { get; set; } = default!;

    protected string? EmailConfirmationLink;
    protected string? StatusMessage;

    [CascadingParameter]
    private HttpContext HttpContext { get; set; } = default!;

    [SupplyParameterFromQuery]
    private string? Email { get; set; }

    [SupplyParameterFromQuery]
    private string? ReturnUrl { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (Email is null)
        {
            RedirectManager.RedirectTo("");
        }

        var user = await UserManager.FindByEmailAsync(Email);
        if (user is null)
        {
            HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
            StatusMessage = "Error finding user for unspecified email";
        }
        else if (EmailSender is IdentityNoOpEmailSender)
        {
            // Once you add a real email sender, you should remove this code that lets you confirm the account
            var userId = await UserManager.GetUserIdAsync(user);
            var code = await UserManager.GenerateEmailConfirmationTokenAsync(user);

            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            EmailConfirmationLink = NavigationManager.GetUriWithQueryParameters(
                NavigationManager.ToAbsoluteUri("Account/ConfirmEmail").AbsoluteUri,
                new Dictionary<string, object?> 
                {
                    ["userId"] = userId, 
                    ["code"] = code, 
                    ["returnUrl"] = ReturnUrl 
                });
        }
    }
}