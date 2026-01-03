namespace Atoms.Core.ExtensionMethods;

public static class DateTimeExtensions
{
    extension(DateTime dateTime)
    {
        public DateTime TruncateToMicroseconds()
        {
            /* Postgres stores dates to microsecond precision,
               so truncate the sub-microsecond portion of the DateTime value */

            var ticks = dateTime.Ticks - (dateTime.Ticks % 10); // 10 ticks = 1 microsecond

            return new DateTime(ticks, dateTime.Kind);
        }
    }
}
