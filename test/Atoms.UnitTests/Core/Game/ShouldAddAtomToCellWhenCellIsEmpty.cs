﻿using static Atoms.Core.Entities.Game;

namespace Atoms.UnitTests.Core.Game;

public class ShouldAddAtomToCellWhenCellIsEmpty
{
    [Test, MethodDataSource(nameof(GetGameStates))]
    public async Task Test(State state)
    {
        var game = new Atoms.Core.Entities.Game(state);
        var cell = game.Board[1, 1];

        game.PlaceAtom(cell);

        await Assert.That(cell.Atoms).IsEqualTo(1);
        await Assert.That(cell.Player).IsEqualTo(game.Players[0]);
    }

    public static IEnumerable<State> GetGameStates()
    {
        yield return ObjectMother.NewGameState;
    }
}