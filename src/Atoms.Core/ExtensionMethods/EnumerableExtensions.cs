namespace Atoms.Core.ExtensionMethods;

internal static class EnumerableExtensions
{
    public static string ToCsv<T>(this IEnumerable<T> values, char separator)
    {
        return string.Join(separator, values);
    }
}
