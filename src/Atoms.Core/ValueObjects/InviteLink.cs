using Flurl;

namespace Atoms.Core.ValueObjects;

public class InviteLink(string code, string baseUrl)
{
    public Uri Url { get; } = new(baseUrl.AppendPathSegments("invites", code));
}