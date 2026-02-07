namespace Atoms.Infrastructure.Middleware;

using Atoms.Infrastructure.Services;
using Microsoft.AspNetCore.Http;

public class SetVisitorIdCookie(
    RequestDelegate next,
    IDateTimeService dateTimeService,
    VisitorIdCookieValueService cookieValueService)
{
    public async Task InvokeAsync(HttpContext context)
    {
        string? acceptHeader = context.Request.Headers.Accept;

        if (!string.IsNullOrEmpty(acceptHeader)
            && acceptHeader.Contains("text/html"))
        {
            var utcNow = dateTimeService.UtcNow;

            if (!cookieValueService.TryGetCookieValue(context, out var visitorIdCookieValue)
                || visitorIdCookieValue.RequiresRenewal(utcNow))
            {
                var newVisitorIdCookieValue = VisitorIdCookieValue.CreateNew(
                    utcNow,
                    visitorIdCookieValue?.Id,
                    visitorIdCookieValue?.Name);

                cookieValueService.SetCookieValue(
                    context, newVisitorIdCookieValue);
            }
        }

        await next(context);
    }
}