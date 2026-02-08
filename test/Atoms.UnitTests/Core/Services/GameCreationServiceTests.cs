using Atoms.Core.Services;

namespace Atoms.UnitTests.Core.Services;

public class GameCreationServiceTests : BaseDbTestFixture
{
    [Test]
    public async Task TestCreateGame()
    {
        using var dbContext = await DbContextFactory.CreateDbContextAsync();

        var visitorServiceExpectations = new IVisitorServiceCreateExpectations();
        visitorServiceExpectations.Setups
            .AddOrUpdate(
                Arg.Is(ObjectMother.VisitorId),
                Arg.Any<CancellationToken?>())
            .ReturnValue(Task.FromResult(ObjectMother.VisitorId));

        await dbContext.Visitors.AddAsync(ObjectMother.VisitorUser);
        await dbContext.SaveChangesAsync();

        var gameDto = ObjectMother.GameDTO();

        var gameCreationService = new GameCreationService(
            visitorServiceExpectations.Instance(),
            (gameId, options, visitorId, userIdentity) => gameDto,
            DbContextFactory);

        var responseGameDto = await gameCreationService.CreateGame(
            ObjectMother.GameMenuOptions,
            ObjectMother.VisitorId,
            ObjectMother.UserIdentity,
            CancellationToken.None);

        await Assert.That(responseGameDto).IsEqualTo(gameDto);

        var gameDtoFromDb = await dbContext.Games.FindAsync(gameDto.Id);

        await Assert.That(gameDtoFromDb).IsNotNull()
             .And.Member(x => x!.Id, x => x.EqualTo(gameDto.Id));
    }
}
