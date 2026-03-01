namespace Atoms.Infrastructure.Email;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Components;
using Atoms.Components.Email;
using Atoms.Core.Interfaces;

public class EmailTemplateService(
    IOptions<EmailSettings> emailSettingsOptions,
    BlazorRenderer blazorRenderer,
    IHttpContextAccessor httpContextAccessor,
    IEmailInliner emailInliner) : IEmailTemplateService
{
    private readonly EmailSettings _emailSettings = emailSettingsOptions.Value;

    public Task<string> GetEmailConfirmationTemplate(string confirmationLink, string userName)
    {
        var parameters = new Dictionary<string, object?>
        {
            { nameof(EmailConfirmationComponent.UserName), userName },
            { nameof(EmailConfirmationComponent.ConfirmationLink), confirmationLink },
            { nameof(EmailConfirmationComponent.FromAddress), _emailSettings.FromAddress },
            { nameof(EmailConfirmationComponent.LogoUrl), GetLogoUrl() }
        };

        return RenderAndInlineEmailAsync<EmailConfirmation>(parameters);
    }

    public Task<string> GetPasswordResetLinkTemplate(string resetLink, string userName)
    {
        var parameters = new Dictionary<string, object?>
        {
            { nameof(PasswordResetLinkComponent.UserName), userName },
            { nameof(PasswordResetLinkComponent.ResetLink), resetLink },
            { nameof(PasswordResetLinkComponent.FromAddress), _emailSettings.FromAddress },
            { nameof(PasswordResetLinkComponent.LogoUrl), GetLogoUrl() }
        };

        return RenderAndInlineEmailAsync<PasswordResetLink>(parameters);
    }

    public Task<string> GetPasswordResetCodeTemplate(string resetCode, string userName)
    {
        var parameters = new Dictionary<string, object?>
        {
            { nameof(PasswordResetCodeComponent.UserName), userName },
            { nameof(PasswordResetCodeComponent.ResetCode), resetCode },
            { nameof(PasswordResetCodeComponent.FromAddress), _emailSettings.FromAddress },
            { nameof(PasswordResetCodeComponent.LogoUrl), GetLogoUrl() }
        };

        return RenderAndInlineEmailAsync<PasswordResetCode>(parameters);
    }

    private async Task<string> RenderAndInlineEmailAsync<T>(Dictionary<string, object?> parameters) where T : IComponent
    {
        var html = await blazorRenderer.RenderComponent<T>(parameters);

        return await emailInliner.InlineCssAsync(html);
    }

    private string GetLogoUrl()
    {
        var baseUrl = GetBaseUrl();

        if (string.IsNullOrEmpty(baseUrl))
        {
            return string.Empty;
        }

        return $"{baseUrl.TrimEnd('/')}/images/logo.png";
    }

    private string GetBaseUrl()
    {
        var httpContext = httpContextAccessor.HttpContext!;
        var request = httpContext.Request;

        return $"{request.Scheme}://{request.Host}";
    }
}
