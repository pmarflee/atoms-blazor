using Microsoft.AspNetCore.DataProtection;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Atoms.Core.DTOs;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public record VisitorIdCookieValue(
    [property: JsonRequired] VisitorId Id,
    string? Name,
    [property: JsonRequired] DateTime IssueDate)
{
    public const int MaxAgeDays = 400;

    public static VisitorIdCookieValue CreateNew(
        DateTime issueDate, VisitorId? visitorId = null, string? name = null)
    {
        visitorId ??= new VisitorId(Guid.CreateVersion7());

        return new(visitorId.Value, name, issueDate);
    }

    public static bool TryParse(
        string value,
        IDataProtector protector,
        [MaybeNullWhen(false)] out VisitorIdCookieValue visitorIdCookieValue)
    {
        try
        {
            var rawCookieValue = protector.Unprotect(value);

            visitorIdCookieValue = JsonSerializer.Deserialize<VisitorIdCookieValue>(rawCookieValue);
        }
        catch
        {
            visitorIdCookieValue = null;
        }

        return visitorIdCookieValue is not null;
    }

    public string Serialize(IDataProtector protector)
    {
        var rawCookieValue = JsonSerializer.Serialize(this);

        return protector.Protect(rawCookieValue);
    }

    public bool RequiresRenewal(DateTime utcNow) 
        => IssueDate.AddDays(MaxAgeDays / 2) < utcNow;
}
