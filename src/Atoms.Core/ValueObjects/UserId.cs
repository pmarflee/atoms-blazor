namespace Atoms.Core.ValueObjects;

public record UserId(string Id)
{
    public static implicit operator string?(UserId? userId) => userId?.Id;
    public static implicit operator UserId?(string? id) =>
        !string.IsNullOrEmpty(id) ? new(id) : null;
    public static UserId FromGuid(Guid guid) => new(guid.ToString());
}
