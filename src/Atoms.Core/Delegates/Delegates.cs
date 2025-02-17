namespace Atoms.Core.Delegates;

public delegate InviteLink CreateInviteLink(Guid gameId,
                                             Guid playerId,
                                             string baseUrl);

public delegate IRandomNumberGenerator CreateRng(int seed, int iterations);

public delegate IPlayerStrategy? CreatePlayerStrategy(PlayerType playerType,
                                                      IRandomNumberGenerator rng);

public delegate Game CreateGame(GameMenuOptions options);