using static Atoms.Core.Entities.Game;

namespace Atoms.UnitTests;

internal static class ObjectMother
{
    public static State NewGameState { get; } =
        new(6,
            10,
            [ new(1, PlayerType.Human, true),
              new(2, PlayerType.Human, false) ],
            []);
}
