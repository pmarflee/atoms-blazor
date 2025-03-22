using Microsoft.AspNetCore.DataProtection;
using System.Text.Json;

namespace Atoms.Core.Serialization;

public class InviteSerializer(IDataProtectionProvider dataProtectionProvider) 
    : IInviteSerializer
{
    public Invite Deserialize(string code)
    {
        var json = Protector.Unprotect(code);

        return JsonSerializer.Deserialize<Invite>(json)!;
    }

    public string Serialize(Invite invite)
    {
        var json = JsonSerializer.Serialize(invite);

        return Protector.Protect(json);
    }

    IDataProtector Protector => 
        dataProtectionProvider.CreateProtector(
            typeof(InviteSerializer).FullName!);
}
