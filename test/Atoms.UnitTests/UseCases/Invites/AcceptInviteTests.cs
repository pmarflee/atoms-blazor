using Atoms.Core.DTOs.Notifications.SignalR;
using Atoms.Core.ValueObjects;
using Atoms.UseCases.Invites.AcceptInvite;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;

namespace Atoms.UnitTests.UseCases.Invites;

public class AcceptInviteTests : BaseDbTestFixture
{
    const string Player_Name = "Bob";

    private INotificationServiceCreateExpectations _notificationServiceExpectations = default!;
    private ILocalStorageUserServiceCreateExpectations _localStorageUserServiceExpectations = default!;
    private IValidatorCreateExpectations<Invite> _validatorExpectations = default!;
    private IDateTimeServiceCreateExpectations _dateServiceCreateExpectations = default!;
    private IServiceScopeFactoryCreateExpectations _serviceScopeFactoryCreateExpectations = default!;
    private IServiceProviderCreateExpectations _serviceProviderCreateExpectations = default!;
    private IServiceScopeCreateExpectations _serviceScopeCreateExpectations = default!;

    protected override Task SetupInternal()
    {
        _notificationServiceExpectations = new INotificationServiceCreateExpectations();
        _localStorageUserServiceExpectations = new ILocalStorageUserServiceCreateExpectations();
        _validatorExpectations = new IValidatorCreateExpectations<Invite>();

        _dateServiceCreateExpectations = new IDateTimeServiceCreateExpectations();
        _dateServiceCreateExpectations.Setups
            .UtcNow.Gets()
            .ReturnValue(ObjectMother.NewLastUpdatedDateUtc);

        return Task.CompletedTask;
    }

    [Test]
    public async Task ShouldReturnFailureResponseWhenInviteIsNotValid()
    {
        var invite = ObjectMother.Invite;

        _validatorExpectations.Setups
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

        _notificationServiceExpectations.Setups
            .NotifyPlayerJoined(
                Arg.Validate<PlayerJoined>(
                    x => x.GameId == ObjectMother.GameId
                    && x.PlayerId == invite.PlayerId),
                CancellationToken.None)
            .ReturnValue(Task.CompletedTask);

        _notificationServiceExpectations.Setups
            .Start(Arg.Any<CancellationToken>())
            .ReturnValue(Task.CompletedTask);

        _notificationServiceExpectations.Setups
            .DisposeAsync()
            .ReturnValue(ValueTask.CompletedTask);

        _localStorageUserServiceExpectations.Setups
            .GetOrAddLocalStorageId(Arg.Any<CancellationToken?>())
            .ReturnValue(Task.FromResult(ObjectMother.LocalStorageId));

        _validatorExpectations.Setups
            .ValidateAsync(Arg.Is(invite), CancellationToken.None)
            .ReturnValue(Task.FromResult(new ValidationResult()));

        await AddGame();

        var handler = CreateHandler();
        var result = await handler.Handle(
            new AcceptInviteRequest(invite, new(userId, Player_Name)),
            CancellationToken.None);

        using var _ = Assert.Multiple();

        await Assert.That(result.Success).IsTrue();

        using var dbContext = await DbContextFactory.CreateDbContextAsync(
            CancellationToken.None);

        var game = await dbContext.GetGameById(ObjectMother.GameId,
                                               CancellationToken.None);

        await Assert.That(game!.LastUpdatedDateUtc)
            .IsEqualTo(ObjectMother.NewLastUpdatedDateUtc);

        var player = game.Players.First(p => p.Id == invite.PlayerId);

        await Assert.That(player.LocalStorageUserId)
            .IsEqualTo(ObjectMother.LocalStorageId.Value);

        await Assert.That(player.UserId).IsEqualTo(userId?.Id);
        await Assert.That(player.LocalStorageUser).IsNotNull();
        await Assert.That(player.LocalStorageUser!.Name).IsEqualTo(Player_Name);
    }

    async Task AddGame()
    {
        using var dbContext = await DbContextFactory.CreateDbContextAsync();

        var gameDto = ObjectMother.GameDTO();

        await dbContext.LocalStorageUsers.AddAsync(ObjectMother.LocalStorageUser);
        await dbContext.Games.AddAsync(gameDto);
        await dbContext.SaveChangesAsync();
    }

    AcceptInviteRequestHandler CreateHandler()
    {
        _serviceProviderCreateExpectations = new IServiceProviderCreateExpectations();
        _serviceProviderCreateExpectations.Setups
            .GetService(Arg.Is(typeof(INotificationService)))
            .ReturnValue(_notificationServiceExpectations.Instance());

        _serviceScopeCreateExpectations = new IServiceScopeCreateExpectations();
        _serviceScopeCreateExpectations.Setups.ServiceProvider.Gets()
            .ReturnValue(_serviceProviderCreateExpectations.Instance());
        _serviceScopeCreateExpectations.Setups.Dispose();

        _serviceScopeFactoryCreateExpectations = new IServiceScopeFactoryCreateExpectations();
        _serviceScopeFactoryCreateExpectations.Setups
            .CreateScope()
            .ReturnValue(_serviceScopeCreateExpectations.Instance());

        return new(_localStorageUserServiceExpectations.Instance(),
            _validatorExpectations.Instance(),
            DbContextFactory,
            _dateServiceCreateExpectations.Instance(),
            _serviceScopeFactoryCreateExpectations.Instance());
    }

    public static IEnumerable<Func<UserId?>> GetTestCaseUsers()
    {
        yield return () => ObjectMother.UserId;
        yield return () => null;
    }
}
