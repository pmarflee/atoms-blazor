namespace Atoms.Infrastructure.Email;

using PreMailer.Net;
using Atoms.Core.Interfaces;

public class EmailInliner : IEmailInliner
{
    public Task<string> InlineCssAsync(string htmlContent)
    {
        try
        {
            var pm = new PreMailer(htmlContent);
            var inlinedHtml = pm.MoveCssInline();

            return Task.FromResult(inlinedHtml.Html);
        }
        catch
        {
            // If inlining fails, return the original HTML
            return Task.FromResult(htmlContent);
        }
    }
}
