namespace Atoms.Core.DTOs;

public class GameDTO
{
    public Guid Id { get; set; }
    public Guid LocalStorageId { get; set; }
    public ColourScheme ColourScheme { get; set; }
    public AtomShape AtomShape { get; set; }
    public ICollection<PlayerDTO> Players { get; } = [];
    public BoardDTO Board { get; set; } = default!;
    public int Move { get; set; }
    public bool IsActive { get; set; }
}

public class PlayerDTO
{
    public Guid Id { get; set; }
    public int Number { get; set; }
    public PlayerType Type { get; set; }
    public string? UserId { get; set; }
    public bool IsWinner { get; set; }
    public Guid GameId { get; set; }
    public GameDTO Game { get; set; } = default!;
}

public class BoardDTO
{
    public required string Data { get; set; }
}
