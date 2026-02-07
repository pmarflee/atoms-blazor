namespace Atoms.UnitTests.Core.DTOs.VisitorIdCookieValue;

using Microsoft.AspNetCore.DataProtection;
using System.Text;

using VisitorIdCookieValueDTO = Atoms.Core.DTOs.VisitorIdCookieValue;

public class TryParseTests
{
    [Test, MethodDataSource(nameof(TestCases))]
    public async Task Test(string protectedValue, string rawValue, bool parsedExpected, VisitorIdCookieValueDTO expected)
    {
        var protectorExpectations = new IDataProtectorCreateExpectations();
        protectorExpectations.Setups.Unprotect(
            Arg.Validate<byte[]>(
                x => x.SequenceEqual(Convert.FromBase64String(protectedValue))))
               .ReturnValue(Encoding.UTF8.GetBytes(rawValue));

        var parsed = VisitorIdCookieValueDTO.TryParse(
            protectedValue,
            protectorExpectations.Instance(),
            out var result);

        using var _ = Assert.Multiple();

        await Assert.That(parsed).IsEqualTo(parsedExpected);
        await Assert.That(result).IsEqualTo(expected);
    }

    public static IEnumerable<Func<(string, string, bool, VisitorIdCookieValueDTO?)>> TestCases()
    {
        var dto = new VisitorIdCookieValueDTO(
            ObjectMother.VisitorId,
            ObjectMother.Username,
            new DateTime(2026, 1, 31, 22, 14, 0));

        var rawValue1 = $"{{\"Id\": \"{dto.Id.Value}\", \"Name\": \"{dto.Name}\", \"IssueDate\": \"{dto.IssueDate:O}\" }}";
        var rawValue1Bytes = Encoding.UTF8.GetBytes(rawValue1);
        var base64EncodedRawValue1 = Convert.ToBase64String(rawValue1Bytes);

        yield return () => (base64EncodedRawValue1, rawValue1, true, dto);

        var rawValue2 = $"{{\"Idx\": \"{dto.Id.Value}\", \"Name\": \"{dto.Name}\", \"IssueDate\": \"{dto.IssueDate:O}\" }}";
        var rawValue2Bytes = Encoding.UTF8.GetBytes(rawValue2);
        var base64EncodedRawValue2 = Convert.ToBase64String(rawValue2Bytes);

        yield return () => (base64EncodedRawValue2, rawValue2, false, null);

        var rawValue3 = $"{{\"Id\": {dto.Id.Value}\", \"IssueDate\": \"{dto.IssueDate:O}\" }}";
        var rawValue3Bytes = Encoding.UTF8.GetBytes(rawValue3);
        var base64EncodedRawValue3 = Convert.ToBase64String(rawValue3Bytes);

        yield return () => (base64EncodedRawValue3, rawValue3, false, null);

        var dto2 = VisitorIdCookieValueDTO.CreateNew(
            new DateTime(2026, 1, 31, 22, 14, 0),
            ObjectMother.VisitorId);

        var rawValue4 = $"{{\"Id\": \"{dto2.Id.Value}\", \"IssueDate\": \"{dto2.IssueDate:O}\" }}";
        var rawValue4Bytes = Encoding.UTF8.GetBytes(rawValue4);
        var base64EncodedRawValue4 = Convert.ToBase64String(rawValue4Bytes);

        yield return () => (base64EncodedRawValue4, rawValue4, true, dto2);
    }
}
