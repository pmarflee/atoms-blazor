using Atoms.Core.DTOs;

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
                    Name = "Paul McCartney",
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
                    Name = "James Last",
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
                    Name = "James Last",
                    AbbreviatedName = "JL"
                },
                new PlayerDTO 
                { 
                    Id = Guid.NewGuid(),
                    PlayerTypeId = PlayerType.Human,
                    Number = 2,
                    Name = "Jimmy Lewis",
                    AbbreviatedName = "JL1"
                }
            ], "JL2");
    }
}
