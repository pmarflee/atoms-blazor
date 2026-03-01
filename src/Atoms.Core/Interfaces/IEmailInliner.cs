namespace Atoms.Core.Interfaces;

public interface IEmailInliner
{
    Task<string> InlineCssAsync(string htmlContent);
}
