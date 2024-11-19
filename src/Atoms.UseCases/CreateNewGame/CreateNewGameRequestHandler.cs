using Atoms.Core.Interfaces;

namespace Atoms.UseCases.CreateNewGame;

public class CreateNewGameRequestHandler(IGameFactory gameFactory) 
    : IRequestHandler<CreateNewGameRequest, CreateNewGameResponse>
{
    public Task<CreateNewGameResponse> Handle(CreateNewGameRequest request,
                                              CancellationToken cancellationToken)
    {
        var game = gameFactory.Create(request.Options);
        
        return Task.FromResult(new CreateNewGameResponse(game));
    }
}
