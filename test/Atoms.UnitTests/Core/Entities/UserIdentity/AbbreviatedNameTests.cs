using UserIdentityEntity = Atoms.Core.Entities.UserIdentity;

namespace Atoms.UnitTests.Core.Entities.UserIdentity;

public class AbbreviatedNameTests
{
    [Test, MethodDataSource(nameof(GetTestData))]
    public async Task TestGenerateAbbreviatedName(UserIdentityEntity userIdentity,
                                            IEnumerable<PlayerDTO>? players,
                                            string? expected)
    {
        await Assert.That(userIdentity.GetAbbreviatedName(players))
            .IsEqualTo(expected);
    }

    public static IEnumerable<Func<(UserIdentityEntity, IEnumerable<PlayerDTO>? players, string?)>> GetTestData()
    {
        yield return () => (new(null), null, null);
        yield return () => (new("John Lennon"), null, "JL");
        yield return () => (new("john_lennon"), null, "JL");
        yield return () => (new("John Lennon"), [], "JL");
        yield return () => (
            new("John Lennon"), 
            [
                new PlayerDTO 
                { 
                    Id = Guid.NewGuid(),
                    PlayerTypeId = PlayerType.Human,
                    Number = 1,
                    AbbreviatedName = "PM"
                }
            ], "JL");
        yield return () => (
            new("John Lennon"), 
            [
                new PlayerDTO 
                { 
                    Id = Guid.NewGuid(),
                    PlayerTypeId = PlayerType.Human,
                    Number = 1,
                    AbbreviatedName = "JL"
                }
            ], "JL1");
        yield return () => (
            new("John Lennon"), 
            [
                new PlayerDTO 
                { 
                    Id = Guid.NewGuid(),
                    PlayerTypeId = PlayerType.Human,
                    Number = 1,
                    AbbreviatedName = "JL"
                },
                new PlayerDTO 
                { 
                    Id = Guid.NewGuid(),
                    PlayerTypeId = PlayerType.Human,
                    Number = 2,
                    AbbreviatedName = "JL1"
                }
            ], "JL2");
    }
}
