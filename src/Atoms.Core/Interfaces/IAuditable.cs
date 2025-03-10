namespace Atoms.Core.Interfaces;

public interface IAuditable
{
    DateTime CreatedDateUtc { get; set; }
    DateTime LastUpdatedDateUtc { get; set; }
}
