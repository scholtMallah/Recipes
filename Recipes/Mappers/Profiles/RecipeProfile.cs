using AutoMapper;
using Recipes.Dtos.HelperTypes;
using Recipes.Dtos.Requests;
using RecipeModel = Recipes.Models.Recipes;

namespace Recipes.Mappers.Profiles;

public class RecipeProfile : Profile
{
    public RecipeProfile()
    {
        CreateMap<RecipeModel, RecipeDto>();
        CreateMap<CreateOrPutRecipeRequest, RecipeModel>();
    }
}
