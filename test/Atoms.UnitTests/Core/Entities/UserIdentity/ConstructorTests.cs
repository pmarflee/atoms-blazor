namespace Atoms.UnitTests.Core.Entities.UserIdentity;

using UserIdentityEntity = Atoms.Core.Entities.UserIdentity;

public class ConstructorTests
{
    [Test]
    [Arguments(" Paul ", "Paul")]
    [Arguments("paul", "paul")]
    [Arguments(" paul", "paul")]
    public async Task NameConstructorShouldTrimWhitespaceFromName(
        string name, string expected)
    {
        var userIdentity = new UserIdentityEntity(name);

        await Assert.That(userIdentity.Name).IsEqualTo(expected);
    }

    [Test]
    [Arguments(" Paul ", "Paul")]
    [Arguments("paul", "paul")]
    [Arguments(" paul", "paul")]
    public async Task UserIdAndNameConstructorShouldTrimWhitespaceFromName(
        string name, string expected)
    {
        var userIdentity = new UserIdentityEntity(null, name);

        await Assert.That(userIdentity.Name).IsEqualTo(expected);
    }
}
