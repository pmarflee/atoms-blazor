using Flurl;
using Flurl.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using Atoms.Core.Entities.Configuration;

namespace Atoms.Infrastructure.Email;

public class MailgunApiEmailSender(IOptions<EmailSettings> emailSettingsOptions) 
    : IEmailSender
{
    private readonly EmailSettings _emailSettings = emailSettingsOptions.Value;

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        await _emailSettings.BaseUrl
            .AppendPathSegments(_emailSettings.Domain, "messages")
            .WithBasicAuth("api", _emailSettings.ApiKey)
            .PostUrlEncodedAsync(new
            {
                from = $"mailgun@{_emailSettings.Domain}",
                to = email,
                subject,
                html = htmlMessage
            });
    }
}
