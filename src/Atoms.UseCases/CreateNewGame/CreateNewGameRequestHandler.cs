namespace Atoms.UseCases.CreateNewGame;

public class CreateNewGameRequestHandler(
    IGameCreationService gameCreationService)
    : IRequestHandler<CreateNewGameRequest, CreateNewGameResponse>
{
    public async Task<CreateNewGameResponse> Handle(
        CreateNewGameRequest request,
        CancellationToken cancellationToken)
    {
        var game = await gameCreationService.CreateGame(
            request.Options,
            request.UserIdentity,
            cancellationToken);

        return new(game);
    }
}
