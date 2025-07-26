using static Atoms.Core.DTOs.GameMenuOptions;

namespace Atoms.UseCases.Menu.GameOptions;

public class CreateGameOptionsRequestHandler(
    IBrowserStorageService browserStorageService) 
    : IRequestHandler<CreateGameOptionsRequest, GameOptionsResponse>
{
    public async Task<GameOptionsResponse> Handle(
        CreateGameOptionsRequest request, CancellationToken cancellationToken)
    {
        var options = await browserStorageService.GetGameMenuOptions();

        if (options is null)
        {
            var players = new List<Player>(request.NumberOfPlayers);

            for (var i = 0; i < request.NumberOfPlayers; i++)
            {
                players.Add(new Player
                {
                    Type = PlayerType.Human,
                    Number = i + 1
                });
            }

            options = new GameMenuOptions
            {
                NumberOfPlayers = players.Count,
                Players = players,
                ColourScheme = ColourScheme.Original,
                AtomShape = AtomShape.Round,
                HasSound = true
            };

            await browserStorageService.SetGameMenuOptions(options);
        }

        return new GameOptionsResponse(options);
    }
}
