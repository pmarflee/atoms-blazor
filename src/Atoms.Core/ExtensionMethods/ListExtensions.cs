namespace Atoms.Core.ExtensionMethods;

internal static class ListExtensions
{
    extension<T>(IList<T> values)
    {
        public T SingleRandom()
        {
            ArgumentNullException.ThrowIfNull(values);

            if (values.Count == 0)
            {
                throw new InvalidOperationException("List contains no values");
            }

            var rng = new Random();
            var index = rng.Next(values.Count);

            return values[index];
        }
    }
}
