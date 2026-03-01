namespace Atoms.Core.Interfaces;

public interface IEmailTemplateService
{
    Task<string> GetEmailConfirmationTemplate(string confirmationLink, string userName);
    Task<string> GetPasswordResetLinkTemplate(string resetLink, string userName);
    Task<string> GetPasswordResetCodeTemplate(string resetCode, string userName);
}
