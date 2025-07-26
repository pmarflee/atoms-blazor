namespace Atoms.Core.Delegates;

public delegate IRandomNumberGenerator CreateRng(int seed, int iterations);

public delegate IPlayerStrategy? CreatePlayerStrategy(PlayerType playerType,
                                                      IRandomNumberGenerator rng);

public delegate Game CreateGame(Guid gameId,
                                GameMenuOptions options,
                                StorageId localStorageId,
                                UserIdentity? userIdentity = null);

public delegate ValueTask<ApplicationUser> GetUserById(UserId userId);

public delegate Guid CreateLocalStorageId();
