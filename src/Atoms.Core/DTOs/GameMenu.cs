namespace Atoms.Core.DTOs;

using static Atoms.Core.Enums.EnumExtensions;

public class GameMenu
{
    public const int MinPlayers = 2;
    public const int MaxPlayers = 4;

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

    public GameMenu(int numberOfPlayers, int maxPlayers)
    {
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

    public class Player
    {
        public required int Number { get; init; }
        public required PlayerType Type { get; set; }
    }
}
