﻿using Atoms.UseCases.PlayerMove;

namespace Atoms.UnitTests.UseCases.PlayerMove;

public class ReturnFailureResponseWhenAtomCannotBePlaced : PlayerMoveAtomTestFixture
{
    [Test]
    public async Task Test()
    {
        var game = ObjectMother.Game(
            active: 2,
            cells: [new(1, 1, 1, 1)]);

        var response = await Handler.Handle(
            new PlayerMoveRequest(game, game.Board[1, 1]),
            CancellationToken.None);

        await Assert.That(response.IsSuccessful).IsFalse();
    }
}
