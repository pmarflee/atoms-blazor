namespace Atoms.Core.Entities.Configuration;

public class EmailSettings
{
    public required string BaseUrl { get; init; }
    public required string Domain { get; init; }
    public required string ApiKey { get; init; }
}
