using FluentValidation;
using Recipes.Dtos.Requests;

namespace Recipes.Validators;

public class PatchRecipeRequestValidator : AbstractValidator<PatchRecipeRequest>
{
    public PatchRecipeRequestValidator()
    {
        RuleFor(x => x)
            .Must(x => x.Ingredients is not null || x.Instructions is not null)
            .WithMessage("At least one field must be provided.");

        When(x => x.Ingredients is not null, () =>
        {
            RuleFor(x => x.Ingredients!)
                .NotEmpty();

            RuleForEach(x => x.Ingredients!)
                .NotEmpty()
                .MaximumLength(500);
        });

        When(x => x.Instructions is not null, () =>
        {
            RuleFor(x => x.Instructions!)
                .NotEmpty();

            RuleForEach(x => x.Instructions!)
                .NotEmpty()
                .MaximumLength(2000);
        });
    }
}
