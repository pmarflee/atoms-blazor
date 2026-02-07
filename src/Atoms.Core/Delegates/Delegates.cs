using Atoms.Core.DTOs.Notifications;

namespace Atoms.Core.Delegates;

public delegate IRandomNumberGenerator CreateRng(int seed, int iterations);

public delegate IPlayerStrategy? CreatePlayerStrategy(PlayerType playerType,
                                                      IRandomNumberGenerator rng);

public delegate GameDTO CreateGame(GameMenuOptions options,
                                   VisitorId visitorId,
                                   UserIdentity? userIdentity = null,
                                   Guid? gameId = null);

public delegate ValueTask<ApplicationUser> GetUserById(UserId userId);
public delegate ValueTask<VisitorDTO> GetVisitorById(VisitorId visitorId);

public delegate Task Notify(GameStateChanged notification);
