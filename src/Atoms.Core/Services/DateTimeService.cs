
namespace Atoms.Core.Services;

public class DateTimeService : IDateTimeService
{
    public DateTime UtcNow => DateTime.UtcNow.TruncateToMicroseconds();
}
