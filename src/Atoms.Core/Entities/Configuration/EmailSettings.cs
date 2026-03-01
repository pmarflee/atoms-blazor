namespace Atoms.Core.Entities.Configuration;

public class EmailSettings
{
    public required string Domain { get; init; }
    public required string ApiKey { get; init; }
    public required string Server { get; init; }
    public required string Username { get; init; }
    public required string Password { get; init; }
    public required string FromName { get; init; }
    public required string FromAddress { get; init; }
}
