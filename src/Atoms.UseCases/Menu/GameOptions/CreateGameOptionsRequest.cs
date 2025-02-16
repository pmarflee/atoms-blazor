namespace Atoms.UseCases.Menu.GameOptions;

public class CreateGameOptionsRequest(string baseUrl) : IRequest<GameOptionsResponse>
{
    public string BaseUrl { get; } = baseUrl;
}
