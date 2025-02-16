
using Atoms.Core.Delegates;

namespace Atoms.UseCases.Menu.GameOptions;

public class CreateGameOptionsRequestHandler(CreateInviteLink inviteLinkFactory) 
    : IRequestHandler<CreateGameOptionsRequest, GameOptionsResponse>
{
    public Task<GameOptionsResponse> Handle(CreateGameOptionsRequest request,
                                            CancellationToken cancellationToken)
    {
        var options = new GameMenuOptions(GameMenuOptions.MinPlayers,
                                          GameMenuOptions.MaxPlayers,
                                          request.BaseUrl,
                                          inviteLinkFactory);

        var response = new GameOptionsResponse(options);

        return Task.FromResult(response);
    }
}
