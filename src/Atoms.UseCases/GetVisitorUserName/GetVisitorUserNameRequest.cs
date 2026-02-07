namespace Atoms.UseCases.GetVisitorUserName;

public record GetVisitorUserNameRequest(VisitorId VisitorId) 
    : IRequest<string?>;
