using Atoms.Core.Serialization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace Atoms.UnitTests.Core.Serialization;

public class InviteSerializerTests
{
    const string Code = "ABC123XXX";
    const string Json = "{\"GameId\":\"5bd0e94d-ed21-4679-8b31-e2c70945c8b4\",\"PlayerId\":\"fe0fa471-ac98-4d1b-825b-4ddf64122022\"}";


    private static readonly byte[] _codeBytes = Encoding.UTF8.GetBytes(Code);

    private IDataProtectionProvider _provider = default!;
    private IDataProtectionProviderCreateExpectations _providerExpectations = default!;
    private IDataProtectorCreateExpectations _protectorExpectations = default!;
    private IDataProtector _protector = default!;
    private InviteSerializer _serializer = default!;

    [Before(Test)]
    public Task Setup()
    {
        _providerExpectations = new IDataProtectionProviderCreateExpectations();
        _protectorExpectations = new IDataProtectorCreateExpectations();

        _protectorExpectations.Methods
            .Protect(Arg.Any<byte[]>())
            .ReturnValue(Encoding.UTF8.GetBytes(Code));

        _protectorExpectations.Methods
            .Unprotect(Arg.Any<byte[]>())
            .ReturnValue(Encoding.UTF8.GetBytes(Json));

        _protector = _protectorExpectations.Instance();

        _providerExpectations.Methods
            .CreateProtector(typeof(InviteSerializer).FullName!)
            .ReturnValue(_protector);

        _provider = _providerExpectations.Instance();

        _serializer = new InviteSerializer(_provider);

        return Task.CompletedTask;
    }

    [Test]
    public async Task TestSerialize()
    {
        var code = _serializer.Serialize(ObjectMother.Invite);

        await Assert.That(code)
            .IsEqualTo(WebEncoders.Base64UrlEncode(_codeBytes));
    }

    [Test]
    public async Task TestDeserialize()
    {
        var invite = _serializer.Deserialize(
            WebEncoders.Base64UrlEncode(_codeBytes));

        await Assert.That(invite).IsEqualTo(ObjectMother.Invite);
    }
}
