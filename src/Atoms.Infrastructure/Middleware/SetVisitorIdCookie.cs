namespace Atoms.Infrastructure.Middleware;

using Atoms.Core.DTOs;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using System.Diagnostics.CodeAnalysis;

public class SetVisitorIdCookie(
    RequestDelegate next,
    IDataProtectionProvider provider,
    IDateTimeService dateTimeService)
{
    const string CookieName = "VisitorId";

    readonly IDataProtector _protector = provider.CreateProtector("Visitor.Id.Protector.V1");

    public async Task InvokeAsync(HttpContext context)
    {
        string? acceptHeader = context.Request.Headers.Accept;

        if (!string.IsNullOrEmpty(acceptHeader)
            && acceptHeader.Contains("text/html"))
        {
            var utcNow = dateTimeService.UtcNow;

            if (!TryGetVisitorIdCookieValue(context, out var visitorIdCookieValue)
                || visitorIdCookieValue.RequiresRenewal(utcNow))
            {
                var newVisitorIdCookieValue = new VisitorIdCookieValue(
                    visitorIdCookieValue?.Id ?? new(Guid.NewGuid()),
                    utcNow);

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
                    newVisitorIdCookieValue.Serialize(_protector),
                    options);
            }
        }

        await next(context);
    }

    bool TryGetVisitorIdCookieValue(
        HttpContext context,
        [MaybeNullWhen(false)] out VisitorIdCookieValue visitorIdCookieValue)
    {
        visitorIdCookieValue = context.Request.Cookies.TryGetValue(CookieName, out var protectedCookieValue)
            && VisitorIdCookieValue.TryParse(protectedCookieValue, _protector, out var visitorIdCookieValue1)
            ? visitorIdCookieValue1
            : null;

        return visitorIdCookieValue is not null;
    }
}