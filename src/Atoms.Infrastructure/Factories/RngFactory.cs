using Atoms.Core.Services;

namespace Atoms.Infrastructure.Factories;

public static class RngFactory
{
    public static IRandomNumberGenerator Create(int seed, int iterations)
    {
        var random = new Random(seed);
        var i = 0;

        while (i++ < iterations)
        {
            random.Next();
        }

        return new RandomNumberGenerator(random, seed, iterations);
    }
}
