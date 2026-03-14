namespace Atoms.UseCases.DeleteUserData;

public record DeleteUserDataRequest(VisitorId VisitorId, UserId? UserId) 
    : IRequest;
