using Atoms.Core.Identity;
using Atoms.Infrastructure.Email;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Atoms.Infrastructure.Identity;

public class IdentityEmailSender(
    IEmailSender emailSender,
    IEmailTemplateService emailTemplateService) : IEmailSender<ApplicationUser>
{
    public async Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink)
    {
        var htmlContent = await emailTemplateService.GetEmailConfirmationTemplate(confirmationLink, user.UserName ?? email);
        await emailSender.SendEmailAsync(email, "Confirm your email", htmlContent);
    }

    public async Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink)
    {
        var htmlContent = await emailTemplateService.GetPasswordResetLinkTemplate(resetLink, user.UserName ?? email);
        await emailSender.SendEmailAsync(email, "Reset your password", htmlContent);
    }

    public async Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode)
    {
        var htmlContent = await emailTemplateService.GetPasswordResetCodeTemplate(resetCode, user.UserName ?? email);
        await emailSender.SendEmailAsync(email, "Reset your password", htmlContent);
    }
}
