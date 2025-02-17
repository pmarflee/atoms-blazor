using Microsoft.AspNetCore.DataProtection;
using System.Text.Json;
using Flurl;

namespace Atoms.Infrastructure.Factories;

public static class InviteLinkFactory
{
    public static InviteLink Create(Guid gameId,
                                    Guid playerId,
                                    string baseUrl,
                                    IDataProtectionProvider dataProtectionProvider)
    {
        var dto = new GameInviteDTO { GameId = gameId, PlayerId = playerId };
        var data = JsonSerializer.Serialize(dto);
        var protector = dataProtectionProvider.CreateProtector(
            typeof(InviteLinkFactory).FullName!);
        var protectedData = protector.Protect(data);
        var url = baseUrl.AppendPathSegments("invites", protectedData);

        return new(url);
    }
}
