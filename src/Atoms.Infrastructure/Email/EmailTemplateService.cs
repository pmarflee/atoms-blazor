namespace Atoms.Infrastructure.Email;

using Microsoft.AspNetCore.Http;
using Atoms.Components.Email;
using Atoms.Core.Interfaces;

public class EmailTemplateService(
    IOptions<EmailSettings> emailSettingsOptions,
    BlazorRenderer blazorRenderer,
    IHttpContextAccessor httpContextAccessor) : IEmailTemplateService
{
    private readonly EmailSettings _emailSettings = emailSettingsOptions.Value;

    public Task<string> GetEmailConfirmationTemplate(string confirmationLink, string userName)
    {
        var parameters = new Dictionary<string, object?>
        {
            { nameof(EmailConfirmationComponent.UserName), userName },
            { nameof(EmailConfirmationComponent.ConfirmationLink), confirmationLink },
            { nameof(EmailConfirmationComponent.FromAddress), _emailSettings.FromAddress },
            { nameof(EmailConfirmationComponent.LogoHtml), GetLogoHtml() }
        };

        return blazorRenderer.RenderComponent<EmailConfirmation>(parameters);
    }

    public Task<string> GetPasswordResetLinkTemplate(string resetLink, string userName)
    {
        var parameters = new Dictionary<string, object?>
        {
            { nameof(PasswordResetLinkComponent.UserName), userName },
            { nameof(PasswordResetLinkComponent.ResetLink), resetLink },
            { nameof(PasswordResetLinkComponent.FromAddress), _emailSettings.FromAddress },
            { nameof(PasswordResetLinkComponent.LogoHtml), GetLogoHtml() }
        };

        return blazorRenderer.RenderComponent<PasswordResetLink>(parameters);
    }

    public Task<string> GetPasswordResetCodeTemplate(string resetCode, string userName)
    {
        var parameters = new Dictionary<string, object?>
        {
            { nameof(PasswordResetCodeComponent.UserName), userName },
            { nameof(PasswordResetCodeComponent.ResetCode), resetCode },
            { nameof(PasswordResetCodeComponent.FromAddress), _emailSettings.FromAddress },
            { nameof(PasswordResetCodeComponent.LogoHtml), GetLogoHtml() }
        };

        return blazorRenderer.RenderComponent<PasswordResetCode>(parameters);
    }

    private string GetLogoHtml()
    {
        var baseUrl = GetBaseUrl();

        if (string.IsNullOrEmpty(baseUrl))
        {
            return string.Empty;
        }

        var logoUrl = $"{baseUrl.TrimEnd('/')}/images/logo.svg";

        return $@"<img src=""{logoUrl}"" alt=""Atoms Logo"" style=""max-width: 150px; height: auto;"" />";
    }

    private string GetBaseUrl()
    {
        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext != null)
        {
            var request = httpContext.Request;
            return $"{request.Scheme}://{request.Host}";
        }

        return string.Empty;
    }
}
