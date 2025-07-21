namespace Atoms.Core.DTOs;

public class GameMenuOptions(List<GameMenuOptions.Player> players,
                             ColourScheme? colourScheme = null,
                             AtomShape? atomShape = null,
                             bool hasSound = true)
{
    public int NumberOfPlayers { get; set; } = players.Count;
    public List<Player> Players { get; } = players;
    public IEnumerable<PlayerType> PlayerTypes { get; } = PlayerType.List.OrderBy(x => x.Value);
    public IEnumerable<ColourScheme> ColourSchemes { get; } = ColourScheme.List.OrderBy(x => x.Value);
    public IEnumerable<AtomShape> AtomShapes { get; } = AtomShape.List.OrderBy(x => x.Value);
    public ColourScheme ColourScheme { get; set; } = colourScheme ?? ColourScheme.Original;
    public AtomShape AtomShape { get; set; } = atomShape ?? AtomShape.Round;
    public bool HasSound { get; set; } = hasSound;

    public static GameMenuOptions CreateForDebug() =>
        new([
                new Player
                {
                    Id = Guid.NewGuid(), Number = 1, Type = PlayerType.Human
                },
                new Player
                {
                    Id = Guid.NewGuid(), Number = 2, Type = PlayerType.Human
                },
            ]);

    public class Player
    {
        public Guid Id { get; init; }
        public required int Number { get; init; }
        public required PlayerType Type { get; set; }
    }
}
