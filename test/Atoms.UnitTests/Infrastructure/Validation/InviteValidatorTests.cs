using Atoms.Core.ValueObjects;
using Atoms.Infrastructure.Validation;
using FluentValidation.TestHelper;

namespace Atoms.UnitTests.Infrastructure.Validation;

public class InviteValidatorTests : BaseDbTestFixture
{
    InviteValidator _validator = default!;

    protected async override Task SetupInternal()
    {
        var gameDto = ObjectMother.GameDTO();

        using var dbContext = await DbContextFactory.CreateDbContextAsync();

        await dbContext.LocalStorageUsers.AddAsync(ObjectMother.LocalStorageUser);
        await dbContext.Games.AddAsync(gameDto);
        await dbContext.SaveChangesAsync();

        var browserStorageServiceExpectations = new IBrowserStorageServiceCreateExpectations();
        browserStorageServiceExpectations.Methods
            .GetOrAddStorageId()
            .ReturnValue(ValueTask.FromResult(ObjectMother.LocalStorageId));

        _validator = new InviteValidator(DbContextFactory,
                                         browserStorageServiceExpectations.Instance());
    }

    [Test]
    public async Task ShouldHaveValidationErrorForGameIdWhenGameDoesNotExist()
    {
        var invite = new Invite(Guid.NewGuid(), ObjectMother.Player1Id);
        var result = await _validator.TestValidateAsync(invite);

        result.ShouldHaveValidationErrorFor(x => x.GameId);
    }

    [Test]
    public async Task ShouldHaveValidationErrorForPlayerIdWhenPlayerDoesNotExist()
    {
        var invite = new Invite(ObjectMother.GameId, Guid.NewGuid());
        var result = await _validator.TestValidateAsync(invite);

        result.ShouldHaveValidationErrorFor(x => x.PlayerId);
    }

    [Test]
    [MethodDataSource(nameof(GetTestArguments))]
    public async Task ShouldHaveValidationErrorForPlayerIdWhenPlayerHasAlreadyAcceptedInvite(UserId? userId, StorageId? localStorageId)
    {
        using var dbContext = await DbContextFactory.CreateDbContextAsync();

        var game = await dbContext.GetGameById(ObjectMother.GameId, CancellationToken.None);
        var player = game!.Players.First(p => p.Id == ObjectMother.Player2Id);

        player.UserId = userId;
        player.LocalStorageUserId = localStorageId?.Value;

        await dbContext.SaveChangesAsync();

        var invite = new Invite(ObjectMother.GameId, ObjectMother.Player2Id);
        var result = await _validator.TestValidateAsync(invite);

        result.ShouldHaveValidationErrorFor(x => x.PlayerId);
    }

    [Test]
    public async Task ShouldNotHaveValidationErrorWhenGameAndPlayerExistsAndPlayerHasNotAcceptedInvite()
    {
        var browserStorageServiceExpectations = new IBrowserStorageServiceCreateExpectations();
        browserStorageServiceExpectations.Methods
            .GetOrAddStorageId()
            .ReturnValue(ValueTask.FromResult(new StorageId(Guid.NewGuid())));

        var validator = new InviteValidator(DbContextFactory,
                                         browserStorageServiceExpectations.Instance());
        var invite = new Invite(ObjectMother.GameId, ObjectMother.Player2Id);
        var result = await validator.ValidateAsync(invite);

        await Assert.That(result.IsValid).IsTrue();
    }

    [Test]
    public async Task ShouldHaveValidationErrorWhenPlayerHasTheSameLocalStorageIdAsThePlayerWhoStartedTheGame()
    {
        var invite = new Invite(ObjectMother.GameId, ObjectMother.Player2Id);
        var result = await _validator.TestValidateAsync(invite);

        result.ShouldHaveValidationErrorFor(x => x.PlayerId);
    }

    public static IEnumerable<Func<(UserId? UserId, StorageId? LocalStorageId)>> GetTestArguments()
    {
        yield return () => (ObjectMother.UserId, null);
        yield return () => (null, ObjectMother.LocalStorageId);
        yield return () => (ObjectMother.UserId, ObjectMother.LocalStorageId);
    }
}
