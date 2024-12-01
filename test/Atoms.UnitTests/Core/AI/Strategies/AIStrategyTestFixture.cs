namespace Atoms.UnitTests.Core.AI.Strategies;

public abstract class AIStrategyTestFixture
{
    internal IRandomNumberGeneratorCreateExpectations RandomNumberGeneratorExpectations { get; private set; } = default!;
    protected IRandomNumberGenerator Rng => RandomNumberGeneratorExpectations.Instance();
    
    [Before(Test)]
    public Task SetupBase()
    {
        RandomNumberGeneratorExpectations = new IRandomNumberGeneratorCreateExpectations();

        return Task.CompletedTask;
    }
}
