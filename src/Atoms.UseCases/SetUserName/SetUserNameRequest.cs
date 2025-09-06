namespace Atoms.UseCases.SetUserName;

public record SetUserNameRequest(UserIdentity UserIdentity, Game? Game = null) 
    : IRequest;
