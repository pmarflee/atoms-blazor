using Atoms.Core.Interfaces;

namespace Atoms.UseCases.CreateNewGame;

public class CreateNewGameRequestHandler(
    Func<PlayerType, IPlayerStrategy> playerStrategyFactory)
    : IRequestHandler<CreateNewGameRequest, CreateNewGameResponse>
{
    const int Rows = 6;
    const int Columns = 10;

    public Task<CreateNewGameResponse> Handle(CreateNewGameRequest request,
                                              CancellationToken cancellationToken)
    {
        var options = request.Options;
        var players = options.Players
            .Take(options.NumberOfPlayers)
            .Select(p => new Game.Player(
                p.Number, p.Type, playerStrategyFactory.Invoke(p.Type)))
            .ToList();

        var game = new Game(Rows,
                        Columns,
                        players,
                        players[0],
                        options.ColourScheme,
                        options.AtomShape);
        
        return Task.FromResult(new CreateNewGameResponse(game));
    }
}
