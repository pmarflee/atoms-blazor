using Flurl;

namespace Atoms.Core.ValueObjects;

public record InviteLink(string Url)
{
    public InviteLink ToWhatsAppLink() => 
        new("https://wa.me/".SetQueryParam("text", Url));

}