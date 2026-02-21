using FluentValidation;
using Recipes.Dtos.Requests;

namespace Recipes.Validators;

public class CreateOrPutRecipeRequestValidator : AbstractValidator<CreateOrPutRecipeRequest>
{
    public CreateOrPutRecipeRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Ingredients)
            .NotNull()
            .NotEmpty();

        RuleForEach(x => x.Ingredients)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(x => x.Instructions)
            .NotNull()
            .NotEmpty();

        RuleForEach(x => x.Instructions)
            .NotEmpty()
            .MaximumLength(2000);
    }
}
