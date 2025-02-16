namespace Atoms.Core.DTOs;

using static Atoms.Core.Enums.EnumExtensions;

public class GameMenuOptions
{
    public const int MinPlayers = 2;
    public const int MaxPlayers = 4;

    public Guid GameId { get; }
    public int NumberOfPlayers { get; set; }
    public List<Player> Players { get; }
    public IEnumerable<KeyValuePair<PlayerType, string>> PlayerTypes { get; } =
        GetValuesDescriptions<PlayerType>();

    public IEnumerable<KeyValuePair<ColourScheme, string>> ColourSchemes { get; } =
        GetValuesDescriptions<ColourScheme>();

    public IEnumerable<KeyValuePair<AtomShape, string>> AtomShapes { get; } =
        GetValuesDescriptions<AtomShape>();

    public ColourScheme ColourScheme { get; set; } = ColourScheme.Original;
    public AtomShape AtomShape { get; set; } = AtomShape.Round;

    public GameMenuOptions(int numberOfPlayers, int maxPlayers, Guid? gameId = null)
    {
        GameId = gameId ?? Guid.NewGuid();
        NumberOfPlayers = numberOfPlayers;
        Players = new List<Player>(maxPlayers);

        for (var i = 0; i < maxPlayers; i++)
        {
            Players.Add(new Player
            {
                Type = PlayerType.Human,
                Number = i + 1,
            });
        }
    }

    GameMenuOptions(List<Player> players)
    {
        NumberOfPlayers = players.Count;
        Players = players;
    }

    public static GameMenuOptions Debug => new(
        [ new Player { Number = 1, Type = PlayerType.Human },
          new Player { Number = 2, Type = PlayerType.Human } ]);

    public class Player
    {
        public Guid Id { get; } = Guid.NewGuid();
        public required int Number { get; init; }
        public required PlayerType Type { get; set; }
        public string? UserId { get; set; }
    }
}
