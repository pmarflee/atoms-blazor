namespace Atoms.UnitTests.Core.DTOs.VisitorIdCookieValue;

using VisitorIdCookieValueDTO = Atoms.Core.DTOs.VisitorIdCookieValue;

public class RequiresRenewalTests
{
    [Test, MethodDataSource(nameof(TestCases))]
    public async Task Test(VisitorIdCookieValueDTO dto, DateTime utcNow, bool expected)
    {
        await Assert.That(dto.RequiresRenewal(utcNow)).IsEqualTo(expected);
    }

    public static IEnumerable<Func<(VisitorIdCookieValueDTO, DateTime, bool)>> TestCases()
    {
        var dto = new VisitorIdCookieValueDTO(
            ObjectMother.VisitorId,
            ObjectMother.Username,
            new DateTime(2026, 1, 31, 22, 14, 0));

        yield return () => (dto, new DateTime(2026, 4, 30, 11, 14, 0), false);
        yield return () => (dto, new DateTime(2026, 8, 20, 10, 30, 0), true);
    }
}
