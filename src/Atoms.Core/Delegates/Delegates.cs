namespace Atoms.Core.Delegates;

public delegate InviteLink CreateInviteLink(Guid gameId,
                                            Guid playerId,
                                            string baseUrl);