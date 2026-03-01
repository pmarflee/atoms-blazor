namespace Atoms.Infrastructure.Email;

using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;

public class SmtpEmailSender(IOptions<EmailSettings> emailSettingsOptions) : IEmailSender
{
    private readonly EmailSettings _emailSettings = emailSettingsOptions.Value;

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(
            _emailSettings.FromName, _emailSettings.FromAddress));
        message.To.Add(new MailboxAddress(string.Empty, email));
        message.Subject = subject;
        message.Sender = new MailboxAddress(
            _emailSettings.FromName, _emailSettings.FromAddress);

        var bodyBuilder = new BodyBuilder { HtmlBody = htmlMessage };
        message.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();

        try
        {
            await client.ConnectAsync(_emailSettings.Server, 587, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);
            await client.SendAsync(message);
        }
        finally
        {
            await client.DisconnectAsync(true);
        }
    }
}
