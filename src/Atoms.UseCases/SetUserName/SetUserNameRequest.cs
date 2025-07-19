namespace Atoms.UseCases.SetUserName;

public class SetUserNameRequest(Game game, UserIdentity userIdentity) : IRequest
{
    public Game Game { get; } = game;
    public UserIdentity UserIdentity { get; } = userIdentity;
}
