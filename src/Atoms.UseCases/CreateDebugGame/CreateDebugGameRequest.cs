namespace Atoms.UseCases.CreateDebugGame;

public class CreateDebugGameRequest(int moves) : IRequest<CreateDebugGameResponse>
{
    public int Moves { get; } = moves;
}
