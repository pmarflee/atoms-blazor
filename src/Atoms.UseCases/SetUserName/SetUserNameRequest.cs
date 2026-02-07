namespace Atoms.UseCases.SetUserName;

public record SetUserNameRequest(VisitorId VisitorId,
                                 UserIdentity UserIdentity,
                                 Game? Game = null) 
    : IRequest;
