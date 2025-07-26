using Atoms.Core.ValueObjects;
using Atoms.UseCases.Invites.AcceptInvite;
using FluentValidation;
using FluentValidation.Results;

namespace Atoms.UnitTests.UseCases.Invites;

public class AcceptInviteTests : BaseDbTestFixture
{
    const string Player_Name = "Bob";

    private IValidatorCreateExpectations<Invite> _validatorExpectations = default!;
    private IBrowserStorageServiceCreateExpectations _browserStorageServiceCreateExpectations = default!;
    private IDateTimeServiceCreateExpectations _dateServiceCreateExpectations = default!;

    protected override Task SetupInternal()
    {
        _validatorExpectations = new IValidatorCreateExpectations<Invite>();

        _browserStorageServiceCreateExpectations = new IBrowserStorageServiceCreateExpectations();
        _browserStorageServiceCreateExpectations.Methods
            .GetOrAddStorageId()
            .ReturnValue(ValueTask.FromResult(ObjectMother.LocalStorageId));

        _dateServiceCreateExpectations = new IDateTimeServiceCreateExpectations();
        _dateServiceCreateExpectations.Properties
            .Getters.UtcNow()
            .ReturnValue(ObjectMother.NewLastUpdatedDateUtc);

        return Task.CompletedTask;
    }

    [Test]
    public async Task ShouldReturnFailureResponseWhenInviteIsNotValid()
    {
        var invite = ObjectMother.Invite;

        _validatorExpectations.Methods
            .ValidateAsync(Arg.Is(invite), CancellationToken.None)
            .ReturnValue(Task.FromResult(new ValidationResult([new ValidationFailure()])));

        var handler = CreateHandler();
        var result = await handler.Handle(
            new AcceptInviteRequest(invite, new(ObjectMother.UserId, Player_Name)),
            CancellationToken.None);

        await Assert.That(result.Success).IsFalse();
    }

    [Test, MethodDataSource(nameof(GetTestCaseUsers))]
    public async Task InviteShouldBeSuccessfullyAcceptedIfItIsValid(UserId? userId)
    {
        var invite = ObjectMother.Invite;

        _validatorExpectations.Methods
            .ValidateAsync(Arg.Is(invite), CancellationToken.None)
            .ReturnValue(Task.FromResult(new ValidationResult()));

        await AddGame();

        _browserStorageServiceCreateExpectations.Methods
            .SetUserName(Player_Name)
            .ReturnValue(ValueTask.CompletedTask)
            .ExpectedCallCount(1);

        var handler = CreateHandler();
        var result = await handler.Handle(
            new AcceptInviteRequest(invite, new(userId, Player_Name)),
            CancellationToken.None);

        using var _ = Assert.Multiple();

        await Assert.That(result.Success).IsTrue();

        using var dbContext = await DbContextFactory.CreateDbContextAsync(
            CancellationToken.None);

        var game = await dbContext.GetGameById(invite.GameId,
                                               CancellationToken.None);

        await Assert.That(game!.LastUpdatedDateUtc)
            .IsEqualTo(ObjectMother.NewLastUpdatedDateUtc);

        var player = game.Players.First(p => p.Id == invite.PlayerId);

        await Assert.That(player.LocalStorageId)
            .IsEqualTo(ObjectMother.LocalStorageId.Value);

        await Assert.That(player.UserId).IsEqualTo(userId?.Id);
        await Assert.That(player.Name).IsEqualTo(Player_Name);

        _browserStorageServiceCreateExpectations.Verify();
    }

    async Task AddGame()
    {
        using var dbContext = await DbContextFactory.CreateDbContextAsync();

        var gameDto = ObjectMother.GameDTO();

        await dbContext.Games.AddAsync(gameDto);
        await dbContext.SaveChangesAsync();
    }

    AcceptInviteRequestHandler CreateHandler() =>
        new(_browserStorageServiceCreateExpectations.Instance(),
            _validatorExpectations.Instance(),
            DbContextFactory,
            _dateServiceCreateExpectations.Instance());

    public static IEnumerable<UserId?> GetTestCaseUsers()
    {
        yield return ObjectMother.UserId;
        yield return null;
    }
}
