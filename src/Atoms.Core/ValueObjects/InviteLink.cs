using Flurl;
using System.Net;

namespace Atoms.Core.ValueObjects;

public class InviteLink
{
    public InviteLink(string code, string baseUrl)
    {
        var inviteUrl = baseUrl.AppendPathSegments("invites", code);
        var message = $"Please join my game!%0A%0A{WebUtility.UrlEncode(inviteUrl)}";

        Url = new("https://wa.me/".SetQueryParam("text", message, true));
    }

    public Uri Url { get; }
}