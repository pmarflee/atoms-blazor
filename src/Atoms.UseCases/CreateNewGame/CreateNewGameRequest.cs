using Atoms.Core.DTOs;

namespace Atoms.UseCases.CreateNewGame;

public class CreateNewGameRequest(GameMenuOptions options) 
    : IRequest<CreateNewGameResponse>
{
    public GameMenuOptions Options { get; } = options;
}
