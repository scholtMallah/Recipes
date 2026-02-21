using FluentValidation;
using Recipes.QueryTypes;

namespace Recipes.Validators;

public class GetAllRecipesQueryValidator : AbstractValidator<GetAllRecipes>
{
    public GetAllRecipesQueryValidator()
    {
        RuleFor(x => x.Offset).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Limit).InclusiveBetween(1, 100);
    }
}
