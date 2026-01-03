namespace Atoms.Core.ExtensionMethods;

internal static class EnumerableExtensions
{
    extension<T>(IEnumerable<T> values)
    {
        public string ToCsv(char separator) => string.Join(separator, values);
    }
}
