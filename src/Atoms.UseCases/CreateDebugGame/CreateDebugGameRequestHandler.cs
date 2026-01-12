namespace Atoms.UseCases.CreateDebugGame;

public class CreateDebugGameRequestHandler(CreateGame gameFactory)
    : IRequestHandler<CreateDebugGameRequest, CreateDebugGameResponse>
{
    public async Task<CreateDebugGameResponse> Handle(
        CreateDebugGameRequest request,
        CancellationToken cancellationToken)
    {
        var options = GameMenuOptions.CreateForDebug();
        var gameDto = gameFactory.Invoke(
            options, request.LocalStorageId, 
            gameId: request.GameId);
        var response = new CreateDebugGameResponse(await gameDto.ToEntity());

        return response;
    }
}
