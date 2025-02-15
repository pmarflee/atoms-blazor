namespace Atoms.UseCases.GetGame;

public class GetGameResponse(bool success, Game? game)
{
    public bool Success { get; } = success;
    public Game? Game { get; } = game;

    public static GetGameResponse Found(Game game) => new(true, game);
    public static GetGameResponse NotFound => new(false, null);
}
