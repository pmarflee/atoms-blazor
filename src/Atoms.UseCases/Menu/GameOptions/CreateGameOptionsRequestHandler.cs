using static Atoms.Core.DTOs.GameMenuOptions;

namespace Atoms.UseCases.Menu.GameOptions;

public class CreateGameOptionsRequestHandler(
    IBrowserStorageService browserStorageService) 
    : IRequestHandler<CreateGameOptionsRequest, GameOptionsResponse>
{
    public async Task<GameOptionsResponse> Handle(
        CreateGameOptionsRequest request, CancellationToken cancellationToken)
    {
        var colourScheme = await browserStorageService.GetColourScheme();
        var atomShape = await browserStorageService.GetAtomShape();
        var hasSound = await browserStorageService.GetSound();
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
            request.GameId, players,
            colourScheme, atomShape, hasSound,
            request.StorageId, request.UserId);

        return new GameOptionsResponse(options);
    }
}
