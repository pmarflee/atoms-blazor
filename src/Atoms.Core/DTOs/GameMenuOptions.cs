namespace Atoms.Core.DTOs;

using static Atoms.Core.Enums.EnumExtensions;

public class GameMenuOptions(Guid gameId,
                             List<GameMenuOptions.Player> players,
                             StorageId localStorageId,
                             UserId? userId)
{
    public Guid GameId { get; } = gameId;
    public int NumberOfPlayers { get; set; } = players.Count;
    public List<Player> Players { get; } = players;
    public StorageId LocalStorageId { get; } = localStorageId;
    public UserId? UserId { get; } = userId;
    public IEnumerable<KeyValuePair<PlayerType, string>> PlayerTypes { get; } =
        GetValuesDescriptions<PlayerType>();

    public IEnumerable<KeyValuePair<ColourScheme, string>> ColourSchemes { get; } =
        GetValuesDescriptions<ColourScheme>();

    public IEnumerable<KeyValuePair<AtomShape, string>> AtomShapes { get; } =
        GetValuesDescriptions<AtomShape>();

    public ColourScheme ColourScheme { get; set; } = ColourScheme.Original;
    public AtomShape AtomShape { get; set; } = AtomShape.Round;

    public InviteLink CreateInviteLink(Player player,
                                       IInviteSerializer inviteSerializer,
                                       string baseUrl)
    {
        var invite = new Invite(GameId, player.Id);
        var code = inviteSerializer.Serialize(invite);

        return new(code, baseUrl);
    }

    public static GameMenuOptions CreateForDebug(
        Guid gameId, StorageId localStorageId, UserId? userId) =>
        new(gameId,
            [
                new Player { Id = Guid.NewGuid(), Number = 1, Type = PlayerType.Human },
                new Player { Id = Guid.NewGuid(), Number = 2, Type = PlayerType.Human },
            ],
            localStorageId,
            userId);

    public class Player
    {
        public Guid Id { get; init; }
        public required int Number { get; init; }
        public required PlayerType Type { get; set; }
        public ApplicationUser? User { get; set; }
    }
}
