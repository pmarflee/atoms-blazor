using FluentValidation;

namespace Atoms.Infrastructure.Validation;

public class UsernameDTOValidator : AbstractValidator<UsernameDTO>
{
    public UsernameDTOValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Username is required")
            .Length(3, 25)
            .WithMessage("Username must be between 3 and 25 characters")
            .Matches("^[A-Za-z0-9]([A-Za-z0-9_ ]*[A-Za-z0-9])?$")
            .WithMessage("Username can only contain letters, numbers, spaces, underscores and dashes, and must start and end with a letter or number")
            .Must(name =>
            {
                var filter = new ProfanityFilter.ProfanityFilter();
                var profanities = filter.DetectAllProfanities(name.ToLower());

                return profanities.Count == 0;
            }).WithMessage("Username contains inappropriate language");
    }
}
