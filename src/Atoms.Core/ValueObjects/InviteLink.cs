using Flurl;

namespace Atoms.Core.ValueObjects;

public class InviteLink
{
    public Uri Url { get; }
    public Uri WhatsAppUrl { get; }

    public InviteLink(string code, string baseUrl)
    {
        Url = new(baseUrl.AppendPathSegments("invites", code));
        WhatsAppUrl = new("https://wa.me/".SetQueryParam("text", Url.ToString()));
    }
}