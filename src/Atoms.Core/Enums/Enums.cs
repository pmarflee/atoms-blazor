using System.ComponentModel;

namespace Atoms.Core.Enums;

public enum GameState
{
    Menu = 1,
    Game,
}

public enum MenuState
{
    Menu = 1,
    About
}

public enum PlayerType
{
    [Description("Human")]
    Human = 1,
    [Description("CPU (E)")]
    CPU_Easy,
    [Description("CPU (M)")]
    CPU_Medium,
    [Description("CPU (H)")]
    CPU_Hard
}

public enum ColourScheme
{
    [Description("Original")]
    Original = 1,
    [Description("Alternate")]
    Alternate
}

public enum AtomShape
{
    [Description("Round")]
    Round = 1,
    [Description("Varied")]
    Varied
}

public enum ExplosionState
{
    None = 0,
    Before = 1,
    After = 2
}

public enum PlayerMoveResult
{
    None = 0,
    Success = 1,
    InvalidMove = 2,
    GameStateHasChanged = 3
}

public enum InviteResult
{
    None = 0,
    Success = 1,
    GameNotFound = 2,
    PlayerNotFound = 3,
    InviteAlreadyAccepted = 4
}
