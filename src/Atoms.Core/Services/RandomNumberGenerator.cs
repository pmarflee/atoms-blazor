using System.Numerics;

namespace Atoms.Core.Services;

public class RandomNumberGenerator(Random random, int seed, int iterations) 
    : IRandomNumberGenerator
{
    public int Seed { get; } = seed;
    public int Iterations { get; private set; } = iterations;

    public int Next(int maxValue) => Increment(() => random.Next(maxValue));
    public int Next(int minValue, int maxValue) => Increment(() => random.Next(minValue, maxValue));
    public double NextDouble() => Increment(random.NextDouble);

    T Increment<T>(Func<T> getNextFunc) where T : INumber<T>
    {
        Iterations++;

        return getNextFunc.Invoke();
    }
}
