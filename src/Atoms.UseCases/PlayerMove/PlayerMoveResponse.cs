namespace Atoms.UseCases.PlayerMove;

public class PlayerMoveResponse(PlayerMoveResult result, bool allowRetry = false)
{
    public bool IsSuccessful { get; } = result == PlayerMoveResult.Ok;
    public PlayerMoveResult Result { get; } = result;
    public bool AllowRetry { get; } = allowRetry;
}
