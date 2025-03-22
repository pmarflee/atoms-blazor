using Atoms.UseCases.Invites.ReadInviteCode;
using FluentValidation;
using FluentValidation.Results;
using System.Security.Cryptography;
using static Atoms.UnitTests.ObjectMother;

namespace Atoms.UnitTests.UseCases.Invites;

public class ReadInviteCodeTests
{
    private IInviteSerializerCreateExpectations _serializerExpectations = default!;
    private IValidatorCreateExpectations<Invite> _validatorExpectations = default!;
    private readonly MockReadInviteCodeRequestLogger _logger = new();

    [Before(Test)]
    public Task Setup()
    {
        _serializerExpectations = new IInviteSerializerCreateExpectations();
        _validatorExpectations = new IValidatorCreateExpectations<Invite>();

        return Task.CompletedTask;
    }

    [Test]
    public async Task ShouldReturnFailureResponseWhenInviteCodeCannotBeRead()
    {
        _serializerExpectations.Methods
            .Deserialize(Arg.Any<string>())
            .Callback(_ => throw new CryptographicException());

        var handler = new ReadInviteCodeRequestHandler(
            _serializerExpectations.Instance(),
            _logger,
            _validatorExpectations.Instance());

        var result = await handler.Handle(
            new ReadInviteCodeRequest("ABC"),
            CancellationToken.None);

        await Assert.That(result.IsSuccessful).IsFalse();
    }

    [Test]
    public async Task ShouldReturnFailureResponseWhenInviteIsNotValid()
    {
        var invite = ObjectMother.Invite;

        _serializerExpectations.Methods
            .Deserialize(Arg.Any<string>())
            .ReturnValue(invite);

        _validatorExpectations.Methods
            .ValidateAsync(Arg.Is(invite), CancellationToken.None)
            .ReturnValue(Task.FromResult(new ValidationResult([new ValidationFailure()])));

        var handler = new ReadInviteCodeRequestHandler(
            _serializerExpectations.Instance(),
            _logger,
            _validatorExpectations.Instance());

        var result = await handler.Handle(
            new ReadInviteCodeRequest("ABC"),
            CancellationToken.None);

        await Assert.That(result.IsSuccessful).IsFalse();
    }

    [Test]
    public async Task ShouldReturnSuccessResponseWhenInviteIsValid()
    {
        var invite = ObjectMother.Invite;

        _serializerExpectations.Methods
            .Deserialize(Arg.Any<string>())
            .ReturnValue(invite);

        _validatorExpectations.Methods
            .ValidateAsync(Arg.Is(invite), CancellationToken.None)
            .ReturnValue(Task.FromResult(new ValidationResult()));

        var handler = new ReadInviteCodeRequestHandler(
            _serializerExpectations.Instance(),
            _logger,
            _validatorExpectations.Instance());

        var result = await handler.Handle(
            new ReadInviteCodeRequest("ABC"),
            CancellationToken.None);

        await Assert.That(result.IsSuccessful).IsTrue();
    }
}
