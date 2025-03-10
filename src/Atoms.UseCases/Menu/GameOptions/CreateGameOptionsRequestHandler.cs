using static Atoms.Core.DTOs.GameMenuOptions;

namespace Atoms.UseCases.Menu.GameOptions;

public class CreateGameOptionsRequestHandler 
    : IRequestHandler<CreateGameOptionsRequest, GameOptionsResponse>
{
    public Task<GameOptionsResponse> Handle(CreateGameOptionsRequest request,
                                            CancellationToken cancellationToken)
    {
        var players = new List<Player>(request.NumberOfPlayers);

        for (var i = 0; i < request.NumberOfPlayers; i++)
        {
            players.Add(new Player
            {
                Id = Guid.NewGuid(),
                Type = PlayerType.Human,
                Number = i + 1 
            });
        }

        var options = new GameMenuOptions(
            request.GameId, players, request.StorageId, request.UserId);
        var response = new GameOptionsResponse(options);

        return Task.FromResult(response);
    }
}
