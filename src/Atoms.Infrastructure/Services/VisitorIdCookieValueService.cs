using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;

namespace Atoms.Infrastructure.Services;

public class VisitorIdCookieValueService(
    IDataProtectionProvider provider,
    IDateTimeService dateTimeService)
{
    const string CookieName = Constants.Cookies.VisitorId;

    readonly IDataProtector _protector = provider.CreateProtector("Visitor.Id.Protector.V1");

    public bool TryGetCookieValue(
        HttpContext context,
        [MaybeNullWhen(false)] out VisitorIdCookieValue cookieValue)
    {
        cookieValue = context.Request.Cookies.TryGetValue(
            CookieName, out var protectedCookieValue)
            && VisitorIdCookieValue.TryParse(protectedCookieValue, _protector, out var visitorIdCookieValue1)
            ? visitorIdCookieValue1
            : null;

        return cookieValue is not null;
    }

    public void SetCookieValue(
        HttpContext context,
        VisitorIdCookieValue cookieValue)
    {
        var options = new CookieOptions
        {
            Path = "/",
            MaxAge = TimeSpan.FromDays(VisitorIdCookieValue.MaxAgeDays),
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax
        };

        context.Response.Cookies.Append(
            CookieName,
            cookieValue.Serialize(_protector),
            options);
    }

    public void SetName(HttpContext context, string name)
    {
        var issueDate = dateTimeService.UtcNow;
        var cookieValue = TryGetCookieValue(context, out var outCookieValue)
            ? (outCookieValue with { Name = name, IssueDate = dateTimeService.UtcNow })
            : VisitorIdCookieValue.CreateNew(
                issueDate, name: name);

        SetCookieValue(context, cookieValue);
    }
}
