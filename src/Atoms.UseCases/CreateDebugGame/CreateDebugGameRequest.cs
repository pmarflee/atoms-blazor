namespace Atoms.UseCases.CreateDebugGame;

public record CreateDebugGameRequest(
    Guid GameId, int Moves, VisitorId VisitorId) 
    : IRequest<CreateDebugGameResponse>;
