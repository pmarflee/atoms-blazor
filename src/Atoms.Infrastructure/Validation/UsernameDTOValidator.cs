using FluentValidation;

namespace Atoms.Infrastructure.Validation;

public class UsernameDTOValidator : AbstractValidator<UsernameDTO>
{
    public UsernameDTOValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Name)
            .NotEmpty()
            .Length(1, 25)
            .Matches("[A-Za-z0-9_ ]+")
            .Must(name =>
            {
                var filter = new ProfanityFilter.ProfanityFilter();
                var profanities = filter.DetectAllProfanities(name);

                return profanities.Count == 0;
            }).WithMessage("Username is invalid");
    }
}
