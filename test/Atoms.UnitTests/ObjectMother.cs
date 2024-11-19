using static Atoms.Core.Entities.Game;

namespace Atoms.UnitTests;

internal static class ObjectMother
{
    public static State NewGameState
    {
        get
        {
            return new State
            {
                Rows = 6,
                Columns = 10,
                Players =
                [
                    new State.Player 
                    {
                        Number = 1, Type = PlayerType.Human, IsActive = true 
                    },
                    new State.Player 
                    { 
                        Number = 2, Type = PlayerType.Human, IsActive = false 
                    }
                ],
                Cells = []
            };
        }
    }
}
