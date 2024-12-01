namespace Atoms.UseCases.PlayerMove;

public class PlayerMoveResponse
{
    private PlayerMoveResponse(bool isSuccessful)
    {
        IsSuccessful = isSuccessful;
    }

    public bool IsSuccessful { get; }

    public static readonly PlayerMoveResponse Success = new(true);
    public static readonly PlayerMoveResponse Failure = new(false);
}
