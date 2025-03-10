
namespace Atoms.UseCases.CreateDebugGame;

public class CreateDebugGameRequestHandler(CreateGame gameFactory)
    : IRequestHandler<CreateDebugGameRequest, CreateDebugGameResponse>
{
    public Task<CreateDebugGameResponse> Handle(CreateDebugGameRequest request,
                                                CancellationToken cancellationToken)
    {
        var options = GameMenuOptions.CreateForDebug();
        var game = gameFactory.Invoke(options);
        var response = new CreateDebugGameResponse(game);

        return Task.FromResult(response);
    }
}
