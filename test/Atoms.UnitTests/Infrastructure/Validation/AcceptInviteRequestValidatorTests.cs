using Atoms.Core.ValueObjects;
using Atoms.Infrastructure.Validation;
using Atoms.UseCases.Invites.AcceptInvite;
using FluentValidation.TestHelper;

namespace Atoms.UnitTests.Infrastructure.Validation;

public class AcceptInviteRequestValidatorTests : BaseDbTestFixture
{
    AcceptInviteRequestValidator _validator = default!;

    protected async override Task SetupInternal()
    {
        var gameDto = ObjectMother.GameDTO();

        using var dbContext = await DbContextFactory.CreateDbContextAsync();

        await dbContext.Visitors.AddAsync(ObjectMother.VisitorUser);
        await dbContext.Games.AddAsync(gameDto);
        await dbContext.SaveChangesAsync();

        _validator = new AcceptInviteRequestValidator(DbContextFactory);
    }

    [Test]
    public async Task ShouldHaveValidationErrorForPlayerIdWhenPlayerDoesNotExist()
    {
        var invite = new Invite(Guid.NewGuid());
        var request = new AcceptInviteRequest(
            invite, ObjectMother.VisitorId, ObjectMother.UserIdentity);
        var result = await _validator.TestValidateAsync(request);

        result.ShouldHaveValidationErrorFor(nameof(Invite.PlayerId));
    }

    [Test]
    [MethodDataSource(nameof(GetTestArguments))]
    public async Task ShouldHaveValidationErrorForPlayerIdWhenPlayerHasAlreadyAcceptedInvite(UserId? userId, VisitorId? visitorId)
    {
        using var dbContext = await DbContextFactory.CreateDbContextAsync();

        var game = await dbContext.GetGameById(ObjectMother.GameId, CancellationToken.None);
        var player = game!.Players.First(p => p.Id == ObjectMother.Player2Id);

        player.UserId = userId;
        player.VisitorId = visitorId?.Value;

        await dbContext.SaveChangesAsync();

        var invite = new Invite(ObjectMother.Player2Id);
        var request = new AcceptInviteRequest(
            invite, ObjectMother.VisitorId, ObjectMother.UserIdentity);
        var result = await _validator.TestValidateAsync(request);

        result.ShouldHaveValidationErrorFor(nameof(Invite.PlayerId));
    }

    [Test]
    public async Task ShouldNotHaveValidationErrorWhenGameAndPlayerExistsAndPlayerHasNotAcceptedInvite()
    {
        var validator = new AcceptInviteRequestValidator(DbContextFactory);
        var invite = new Invite(ObjectMother.Player2Id);
        var request = new AcceptInviteRequest(
            invite, new(Guid.NewGuid()), ObjectMother.UserIdentity);
        var result = await validator.ValidateAsync(request);

        await Assert.That(result.IsValid).IsTrue();
    }

    [Test]
    public async Task ShouldHaveValidationErrorWhenPlayerHasTheSameVisitorIdAsThePlayerWhoStartedTheGame()
    {
        var invite = new Invite(ObjectMother.Player2Id);
        var request = new AcceptInviteRequest(
            invite, ObjectMother.VisitorId, ObjectMother.UserIdentity);
        var result = await _validator.TestValidateAsync(request);

        result.ShouldHaveValidationErrorFor(nameof(Invite.PlayerId));
    }

    public static IEnumerable<Func<(UserId? UserId, VisitorId? VisitorId)>> GetTestArguments()
    {
        yield return () => (ObjectMother.UserId, null);
        yield return () => (null, ObjectMother.VisitorId);
        yield return () => (ObjectMother.UserId, ObjectMother.VisitorId);
    }
}
