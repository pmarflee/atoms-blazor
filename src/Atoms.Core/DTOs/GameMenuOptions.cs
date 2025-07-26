using Ardalis.SmartEnum.SystemTextJson;
using System.Text.Json.Serialization;

namespace Atoms.Core.DTOs;

public class GameMenuOptions
{
    public int NumberOfPlayers { get; set; }

    public List<Player> Players { get; set; } = default!;

    [JsonConverter(typeof(SmartEnumNameConverter<ColourScheme, int>))]
    public ColourScheme ColourScheme { get; set; } = default!;

    [JsonConverter(typeof(SmartEnumNameConverter<AtomShape, int>))]
    public AtomShape AtomShape { get; set; } = default!;

    public bool HasSound { get; set; }

    public static GameMenuOptions CreateForDebug() =>
        new()
        {
            NumberOfPlayers = 2,
            Players = 
            [
                new() { Number = 1, Type = PlayerType.Human },
                new() { Number = 2, Type = PlayerType.Human },
            ]
        };

    public class Player
    {
        public required int Number { get; init; }

        [JsonConverter(typeof(SmartEnumNameConverter<PlayerType, int>))]
        public required PlayerType Type { get; set; }
    }
}
