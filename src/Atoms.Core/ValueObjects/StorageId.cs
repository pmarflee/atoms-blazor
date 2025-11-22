namespace Atoms.Core.ValueObjects;

public record StorageId(Guid Value)
{
    public static implicit operator StorageId(Guid id) => new(id);
}
