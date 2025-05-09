﻿namespace Atoms.UseCases.PlayerMove;

public class PlayerMoveResponse(PlayerMoveResult result)
{
    public bool IsSuccessful { get; } = result == PlayerMoveResult.Success;
    public PlayerMoveResult Result { get; } = result;
}
