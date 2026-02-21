using FluentValidation;
using Recipes.QueryTypes;

namespace Recipes.Validators;

public class SearchRecipesValidator : AbstractValidator<SearchRecipes>
{
    public SearchRecipesValidator()
    {
        RuleFor(x => x.Query)
            .Must(q => !string.IsNullOrWhiteSpace(q))
            .WithMessage("Query is required.");

        RuleFor(x => x.Offset)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.Limit)
            .InclusiveBetween(1, 100);
    }
}