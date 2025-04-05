namespace Atoms.Core.DTOs;

using static Atoms.Core.Enums.EnumExtensions;

public class GameMenuOptions(Guid gameId,
                             List<GameMenuOptions.Player> players,
                             ColourScheme colourScheme = ColourScheme.Original,
                             AtomShape atomShape = AtomShape.Round,
                             bool hasSound = true,
                             StorageId? localStorageId = null,
                             UserId? userId = null)
{
    public Guid GameId { get; } = gameId;
    public int NumberOfPlayers { get; set; } = players.Count;
    public List<Player> Players { get; } = players;
    public StorageId? LocalStorageId { get; } = localStorageId;
    public UserId? UserId { get; } = userId;
    public IEnumerable<PlayerType> PlayerTypes { get; } = PlayerType.List.OrderBy(pt => pt.Value);

    public IEnumerable<KeyValuePair<ColourScheme, string>> ColourSchemes { get; } =
        GetValuesDescriptions<ColourScheme>();

    public IEnumerable<KeyValuePair<AtomShape, string>> AtomShapes { get; } =
        GetValuesDescriptions<AtomShape>();

    public ColourScheme ColourScheme { get; set; } = colourScheme;
    public AtomShape AtomShape { get; set; } = atomShape;
    public bool HasSound { get; set; } = hasSound;

    public InviteLink CreateInviteLink(Player player,
                                       IInviteSerializer inviteSerializer,
                                       string baseUrl)
    {
        var invite = new Invite(GameId, player.Id);
        var code = inviteSerializer.Serialize(invite);

        return new(code, baseUrl);
    }

    public static GameMenuOptions CreateForDebug() =>
        new(Guid.NewGuid(),
            [
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
        public ApplicationUser? User { get; set; }
    }
}
