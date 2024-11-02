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
    [Description("CPU (Easy)")]
    CPU_Easy,
    [Description("CPU (Medium)")]
    CPU_Medium,
    [Description("CPU (Hard)")]
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