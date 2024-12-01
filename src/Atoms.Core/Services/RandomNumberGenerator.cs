using Atoms.Core.Interfaces;

namespace Atoms.Core.Services;

public class RandomNumberGenerator(Random random) : IRandomNumberGenerator
{
    public int Next(int maxValue) => random.Next(maxValue);
    public int Next(int minValue, int maxValue) => random.Next(minValue, maxValue);
    public double NextDouble() => random.NextDouble();
}
