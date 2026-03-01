using Atoms.Infrastructure.Validation;
using FluentValidation.TestHelper;

namespace Atoms.UnitTests.Infrastructure.Validation;

public class UsernameDTOValidatorTests
{
    UsernameDTOValidator _validator = default!;

    [Before(Test)]
    public Task Setup()
    {
        _validator = new UsernameDTOValidator();
        return Task.CompletedTask;
    }

    #region NotEmpty Tests

    [Test]
    public async Task ShouldHaveValidationErrorWhenNameIsNull()
    {
        var dto = new UsernameDTO { Name = null };
        var result = await _validator.TestValidateAsync(dto);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public async Task ShouldHaveValidationErrorWhenNameIsEmpty()
    {
        var dto = new UsernameDTO { Name = string.Empty };
        var result = await _validator.TestValidateAsync(dto);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public async Task ShouldHaveValidationErrorWhenNameIsOnlyWhitespace()
    {
        var dto = new UsernameDTO { Name = "   " };
        var result = await _validator.TestValidateAsync(dto);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    #endregion

    #region Length Tests

    [Test]
    public async Task ShouldHaveValidationErrorWhenNameIsLongerThan25Characters()
    {
        var dto = new UsernameDTO { Name = "a".PadRight(26, 'a') };
        var result = await _validator.TestValidateAsync(dto);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public async Task ShouldBeValidWhenNameIsExactly25Characters()
    {
        var dto = new UsernameDTO { Name = "a".PadRight(25, 'a') };
        var result = await _validator.TestValidateAsync(dto);

        await Assert.That(result.IsValid).IsTrue();
    }

    [Test]
    public async Task ShouldHaveValidationErrorWhenNameIsShorterThan3Characters()
    {
        var dto = new UsernameDTO { Name = "ab" };
        var result = await _validator.TestValidateAsync(dto);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public async Task ShouldBeValidWhenNameIsExactly3Characters()
    {
        var dto = new UsernameDTO { Name = "abc" };
        var result = await _validator.TestValidateAsync(dto);

        await Assert.That(result.IsValid).IsTrue();
    }

    #endregion

    #region Regex - Allowed Characters Tests

    [Test]
    public async Task ShouldBeValidWithLettersOnly()
    {
        var dto = new UsernameDTO { Name = "abcd" };
        var result = await _validator.TestValidateAsync(dto);

        await Assert.That(result.IsValid).IsTrue();
    }

    [Test]
    public async Task ShouldBeValidWithNumbersOnly()
    {
        var dto = new UsernameDTO { Name = "1234" };
        var result = await _validator.TestValidateAsync(dto);

        await Assert.That(result.IsValid).IsTrue();
    }

    [Test]
    public async Task ShouldBeValidWithUnderscores()
    {
        var dto = new UsernameDTO { Name = "user_name" };
        var result = await _validator.TestValidateAsync(dto);

        await Assert.That(result.IsValid).IsTrue();
    }

    [Test]
    public async Task ShouldBeValidWithSpaces()
    {
        var dto = new UsernameDTO { Name = "user name" };
        var result = await _validator.TestValidateAsync(dto);

        await Assert.That(result.IsValid).IsTrue();
    }

    [Test]
    public async Task ShouldBeValidWithMixedAllowedCharacters()
    {
        var dto = new UsernameDTO { Name = "User_Name 123" };
        var result = await _validator.TestValidateAsync(dto);

        await Assert.That(result.IsValid).IsTrue();
    }

    [Test]
    public async Task ShouldHaveValidationErrorWhenContainsSpecialCharacters()
    {
        var dto = new UsernameDTO { Name = "user@name" };
        var result = await _validator.TestValidateAsync(dto);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public async Task ShouldHaveValidationErrorWhenContainsHyphen()
    {
        var dto = new UsernameDTO { Name = "user-name" };
        var result = await _validator.TestValidateAsync(dto);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public async Task ShouldHaveValidationErrorWhenContainsPeriod()
    {
        var dto = new UsernameDTO { Name = "user.name" };
        var result = await _validator.TestValidateAsync(dto);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    #endregion

    #region Regex - Start/End with Alphanumeric Tests

    [Test]
    public async Task ShouldHaveValidationErrorWhenStartsWithUnderscore()
    {
        var dto = new UsernameDTO { Name = "_username" };
        var result = await _validator.TestValidateAsync(dto);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public async Task ShouldHaveValidationErrorWhenStartsWithSpace()
    {
        var dto = new UsernameDTO { Name = " username" };
        var result = await _validator.TestValidateAsync(dto);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public async Task ShouldHaveValidationErrorWhenEndsWithUnderscore()
    {
        var dto = new UsernameDTO { Name = "username_" };
        var result = await _validator.TestValidateAsync(dto);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public async Task ShouldHaveValidationErrorWhenEndsWithSpace()
    {
        var dto = new UsernameDTO { Name = "username " };
        var result = await _validator.TestValidateAsync(dto);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public async Task ShouldBeValidWhenStartsAndEndsWithLetters()
    {
        var dto = new UsernameDTO { Name = "aUsername_123b" };
        var result = await _validator.TestValidateAsync(dto);

        await Assert.That(result.IsValid).IsTrue();
    }

    [Test]
    public async Task ShouldBeValidWhenStartsAndEndsWithNumbers()
    {
        var dto = new UsernameDTO { Name = "1username_2" };
        var result = await _validator.TestValidateAsync(dto);

        await Assert.That(result.IsValid).IsTrue();
    }

    #endregion

    #region Alphanumeric Character Requirement Tests

    [Test]
    public async Task ShouldHaveValidationErrorWhenContainsOnlyUnderscores()
    {
        var dto = new UsernameDTO { Name = "_____" };
        var result = await _validator.TestValidateAsync(dto);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public async Task ShouldBeValidWhenContainsAtLeastOneAlphanumericCharacter()
    {
        var dto = new UsernameDTO { Name = "a_b" };
        var result = await _validator.TestValidateAsync(dto);

        await Assert.That(result.IsValid).IsTrue();
    }

    #endregion

    #region CascadeMode Tests

    [Test]
    public async Task ShouldStopValidationAtFirstErrorDueToRuleLevelCascadeMode()
    {
        var dto = new UsernameDTO { Name = null };
        var result = await _validator.TestValidateAsync(dto);

        // Should have at least one validation error
        await Assert.That(result.Errors.Count).IsGreaterThanOrEqualTo(1);
    }

    #endregion

    #region Profanity Tests

    [Test]
    public async Task ShouldBeValidWhenNameIsProfanityFree()
    {
        var dto = new UsernameDTO { Name = "JohnDoe" };
        var result = await _validator.TestValidateAsync(dto);

        await Assert.That(result.IsValid).IsTrue();
    }

    [Test]
    public async Task ShouldBeValidWithCommonCleanWords()
    {
        var dto = new UsernameDTO { Name = "FriendlyPlayer" };
        var result = await _validator.TestValidateAsync(dto);

        await Assert.That(result.IsValid).IsTrue();
    }

    [Test]
    public async Task ShouldBeValidWithNumbersAndCleanText()
    {
        var dto = new UsernameDTO { Name = "Player123" };
        var result = await _validator.TestValidateAsync(dto);

        await Assert.That(result.IsValid).IsTrue();
    }

    [Test]
    public async Task ShouldBeValidWithUnderscoresAndCleanText()
    {
        var dto = new UsernameDTO { Name = "Clean_Player_Name" };
        var result = await _validator.TestValidateAsync(dto);

        await Assert.That(result.IsValid).IsTrue();
    }

    [Test]
    public async Task ShouldBeValidWithSpacesAndCleanText()
    {
        var dto = new UsernameDTO { Name = "Nice Player Name" };
        var result = await _validator.TestValidateAsync(dto);

        await Assert.That(result.IsValid).IsTrue();
    }

    [Test]
    public async Task ShouldHaveValidationErrorWhenNameContainsFuck()
    {
        var dto = new UsernameDTO { Name = "fuck" };
        var result = await _validator.TestValidateAsync(dto);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Username contains inappropriate language");
    }

    [Test]
    public async Task ShouldHaveValidationErrorWhenNameContainsFuckInMixedCase()
    {
        var dto = new UsernameDTO { Name = "FuCk" };
        var result = await _validator.TestValidateAsync(dto);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Username contains inappropriate language");
    }

    [Test]
    public async Task ShouldHaveValidationErrorWhenNameContainsFuckInUpperCase()
    {
        var dto = new UsernameDTO { Name = "FUCK" };
        var result = await _validator.TestValidateAsync(dto);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Username contains inappropriate language");
    }

    [Test]
    public async Task ShouldHaveValidationErrorWhenNameContainsCunt()
    {
        var dto = new UsernameDTO { Name = "cunt" };
        var result = await _validator.TestValidateAsync(dto);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Username contains inappropriate language");
    }

    [Test]
    public async Task ShouldHaveValidationErrorWhenNameContainsCuntInMixedCase()
    {
        var dto = new UsernameDTO { Name = "CuNt" };
        var result = await _validator.TestValidateAsync(dto);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Username contains inappropriate language");
    }

    [Test]
    public async Task ShouldHaveValidationErrorWhenNameContainsCuntInUpperCase()
    {
        var dto = new UsernameDTO { Name = "CUNT" };
        var result = await _validator.TestValidateAsync(dto);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Username contains inappropriate language");
    }

    [Test]
    public async Task ShouldHaveValidationErrorWhenNameContainsBitch()
    {
        var dto = new UsernameDTO { Name = "bitch" };
        var result = await _validator.TestValidateAsync(dto);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Username contains inappropriate language");
    }

    [Test]
    public async Task ShouldHaveValidationErrorWhenNameContainsBitchInMixedCase()
    {
        var dto = new UsernameDTO { Name = "BiTcH" };
        var result = await _validator.TestValidateAsync(dto);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Username contains inappropriate language");
    }

    [Test]
    public async Task ShouldHaveValidationErrorWhenNameContainsDamn()
    {
        var dto = new UsernameDTO { Name = "damn" };
        var result = await _validator.TestValidateAsync(dto);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Username contains inappropriate language");
    }

    [Test]
    public async Task ShouldHaveValidationErrorWhenNameContainsDamnInMixedCase()
    {
        var dto = new UsernameDTO { Name = "DaMn" };
        var result = await _validator.TestValidateAsync(dto);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Username contains inappropriate language");
    }

    [Test]
    public async Task ShouldHaveValidationErrorWhenNameContainsHell()
    {
        var dto = new UsernameDTO { Name = "hell" };
        var result = await _validator.TestValidateAsync(dto);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Username contains inappropriate language");
    }

    [Test]
    public async Task ShouldHaveValidationErrorWhenNameContainsHellInMixedCase()
    {
        var dto = new UsernameDTO { Name = "HeLL" };
        var result = await _validator.TestValidateAsync(dto);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Username contains inappropriate language");
    }

    [Test]
    public async Task ShouldHaveValidationErrorWhenNameContainsPiss()
    {
        var dto = new UsernameDTO { Name = "piss" };
        var result = await _validator.TestValidateAsync(dto);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Username contains inappropriate language");
    }

    [Test]
    public async Task ShouldHaveValidationErrorWhenNameContainsPissInMixedCase()
    {
        var dto = new UsernameDTO { Name = "PiSs" };
        var result = await _validator.TestValidateAsync(dto);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Username contains inappropriate language");
    }

    [Test]
    public async Task ShouldHaveValidationErrorWhenNameEmbedsProfanityInMiddle()
    {
        var dto = new UsernameDTO { Name = "Player_fuck_123" };
        var result = await _validator.TestValidateAsync(dto);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Username contains inappropriate language");
    }

    [Test]
    public async Task ShouldHaveValidationErrorWhenNameEmbedsProfanityInMiddleWithMixedCase()
    {
        var dto = new UsernameDTO { Name = "Player_FuCk_123" };
        var result = await _validator.TestValidateAsync(dto);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Username contains inappropriate language");
    }

    [Test]
    public async Task ShouldHaveValidationErrorWhenNameStartsWithProfanity()
    {
        var dto = new UsernameDTO { Name = "damned_player" };
        var result = await _validator.TestValidateAsync(dto);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Username contains inappropriate language");
    }

    [Test]
    public async Task ShouldHaveValidationErrorWhenNameStartsWithProfanityInMixedCase()
    {
        var dto = new UsernameDTO { Name = "DaMnEd_player" };
        var result = await _validator.TestValidateAsync(dto);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Username contains inappropriate language");
    }

    [Test]
    public async Task ShouldHaveValidationErrorWhenNameEndsWithProfanity()
    {
        var dto = new UsernameDTO { Name = "player_piss" };
        var result = await _validator.TestValidateAsync(dto);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Username contains inappropriate language");
    }

    [Test]
    public async Task ShouldHaveValidationErrorWhenNameEndsWithProfanityInMixedCase()
    {
        var dto = new UsernameDTO { Name = "player_PiSs" };
        var result = await _validator.TestValidateAsync(dto);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Username contains inappropriate language");
    }

    #endregion

    #region Edge Case Tests

    [Test]
    public async Task ShouldHaveValidationErrorWithOnlyTwoCharacters()
    {
        var dto = new UsernameDTO { Name = "AB" };
        var result = await _validator.TestValidateAsync(dto);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public async Task ShouldBeValidWithMinimumThreeCharacters()
    {
        var dto = new UsernameDTO { Name = "ABC" };
        var result = await _validator.TestValidateAsync(dto);

        await Assert.That(result.IsValid).IsTrue();
    }

    [Test]
    public async Task ShouldBeValidWithMixedCaseLetters()
    {
        var dto = new UsernameDTO { Name = "UserName123" };
        var result = await _validator.TestValidateAsync(dto);

        await Assert.That(result.IsValid).IsTrue();
    }

    [Test]
    public async Task ShouldBeValidWith25CharactersIncludingSpecialAllowedChars()
    {
        var dto = new UsernameDTO { Name = "User_Name 1234567890123" };
        var result = await _validator.TestValidateAsync(dto);

        await Assert.That(result.IsValid).IsTrue();
    }

    #endregion
}
