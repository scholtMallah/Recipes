using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Recipes.Controllers;
using Recipes.Dtos.HelperTypes;
using Recipes.Dtos.Requests;
using Recipes.Dtos.Responses;
using Recipes.Interfaces;
using Recipes.QueryTypes;
using Xunit;

using RecipeModel = Recipes.Models.Recipes;

namespace Recipes.Tests.Controllers;

public class RecipesControllerTests
{
    private static RecipesController CreateController(
        Mock<IRecipesService> serviceMock,
        Mock<IMapper>? mapperMock = null)
    {
        mapperMock ??= new Mock<IMapper>();
        return new RecipesController(serviceMock.Object, mapperMock.Object);
    }

    [Fact]
    public async Task GetRecipeNames_ReturnsOk()
    {
        var svc = new Mock<IRecipesService>();
        svc.Setup(s => s.GetRecipeNames())
            .ReturnsAsync(new List<string> { "a", "b" });

        var controller = CreateController(svc);

        var result = await controller.GetRecipeNames();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var body = Assert.IsType<GetRecipeNamesResponse>(ok.Value);

        Assert.Equal(2, body.RecipeNames.Count);
    }

    [Fact]
    public async Task Post_Returns201()
    {
        var svc = new Mock<IRecipesService>();
        var mapper = new Mock<IMapper>();

        mapper.Setup(m => m.Map<RecipeModel>(It.IsAny<CreateOrPutRecipeRequest>()))
              .Returns(new RecipeModel
              {
                  Name = "x",
                  Ingredients = new List<string> { "i" },
                  Instructions = new List<string> { "s" }
              });

        svc.Setup(s => s.CreateRecipe(It.IsAny<RecipeModel>()))
           .Returns(Task.CompletedTask);

        var controller = CreateController(svc, mapper);

        var res = await controller.Create(new CreateOrPutRecipeRequest
        {
            Name = "x",
            Ingredients = new List<string> { "i" },
            Instructions = new List<string> { "s" }
        });

        var status = Assert.IsType<StatusCodeResult>(res);
        Assert.Equal(StatusCodes.Status201Created, status.StatusCode);
    }

    [Fact]
    public async Task Put_Returns204()
    {
        var svc = new Mock<IRecipesService>();
        var mapper = new Mock<IMapper>();

        mapper.Setup(m => m.Map<RecipeModel>(It.IsAny<CreateOrPutRecipeRequest>()))
              .Returns(new RecipeModel
              {
                  Name = "x",
                  Ingredients = new List<string> { "i" },
                  Instructions = new List<string> { "s" }
              });

        svc.Setup(s => s.UpdateRecipe(It.IsAny<string>(), It.IsAny<RecipeModel>()))
           .Returns(Task.CompletedTask);

        var controller = CreateController(svc, mapper);

        var res = await controller.Update(new CreateOrPutRecipeRequest
        {
            Name = "x",
            Ingredients = new List<string> { "i" },
            Instructions = new List<string> { "s" }
        });

        Assert.IsType<NoContentResult>(res);
    }

    [Fact]
    public async Task GetRecipe_ReturnsOk_WithMappedDto()
    {
        var svc = new Mock<IRecipesService>();
        var mapper = new Mock<IMapper>();

        var model = new RecipeModel
        {
            Name = "test",
            Ingredients = new List<string> { "i" },
            Instructions = new List<string> { "s" }
        };

        var dto = new RecipeDto
        {
            Name = "test",
            Ingredients = new List<string> { "i" },
            Instructions = new List<string> { "s" }
        };

        svc.Setup(s => s.GetRecipe("test")).ReturnsAsync(model);
        mapper.Setup(m => m.Map<RecipeDto>(model)).Returns(dto);

        var controller = CreateController(svc, mapper);

        var result = await controller.GetRecipe("test");

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var body = Assert.IsType<GetRecipeResponse>(ok.Value);

        Assert.Equal("test", body.Recipe.Name);
    }

    [Fact]
    public async Task Search_ReturnsOk_WithPagedResponse()
    {
        var svc = new Mock<IRecipesService>();
        var mapper = new Mock<IMapper>();

        var models = new List<RecipeModel>
    {
        new RecipeModel { Name = "garlicPasta", Ingredients = new List<string>{ "4 cloves garlic" }, Instructions = new List<string>{ "x" } }
    };

        svc.Setup(s => s.SearchRecipes(It.IsAny<SearchRecipes>()))
           .ReturnsAsync((models, 1, 10, 1));

        mapper.Setup(m => m.Map<RecipeDto>(It.IsAny<RecipeModel>()))
              .Returns((RecipeModel m) => new RecipeDto
              {
                  Name = m.Name,
                  Ingredients = m.Ingredients,
                  Instructions = m.Instructions
              });

        var controller = new Recipes.Controllers.RecipesController(svc.Object, mapper.Object);

        var result = await controller.Search(new SearchRecipes { Query = "garlic", Offset = 0, Limit = 10 });

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var body = Assert.IsType<GetAllRecipesResponse>(ok.Value);

        Assert.Single(body.Recipes);
        Assert.Equal("garlicPasta", body.Recipes[0].Name);
        Assert.Equal(1, body.Page);
    }
}