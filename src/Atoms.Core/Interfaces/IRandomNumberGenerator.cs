namespace Atoms.Core.Interfaces;

public interface IRandomNumberGenerator
{
    int Seed { get; }
    int Iterations { get; }

    int Next(int maxValue);
    int Next(int minValue, int maxValue);
    double NextDouble();
}
