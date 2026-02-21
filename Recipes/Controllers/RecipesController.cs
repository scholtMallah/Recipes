using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Recipes.Dtos.HelperTypes;
using Recipes.Dtos.Requests;
using Recipes.Dtos.Responses;
using Recipes.Interfaces;
using RecipeModel = Recipes.Models.Recipes;
using Recipes.QueryTypes;
using Microsoft.AspNetCore.Authorization;

namespace Recipes.Controllers;

[ApiController]
[Route("recipes")]
public class RecipesController : ControllerBase
{
    private readonly IRecipesService _service;
    private readonly IMapper _mapper;

    public RecipesController(IRecipesService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<GetRecipeNamesResponse>> GetRecipeNames()
    {
        var names = await _service.GetRecipeNames();
        return Ok(new GetRecipeNamesResponse(names.ToList()));
    }

    [HttpGet("details/{name}")]
    [AllowAnonymous]
    public async Task<ActionResult<GetRecipeIngredientSummaryResponse>> GetRecipeDetails(string name)
    {
        var details = await _service.GetRecipeIngredientSummary(name);
        return Ok(new GetRecipeIngredientSummaryResponse(details));
    }

    [HttpGet("{name}")]
    [AllowAnonymous]
    public async Task<ActionResult<GetRecipeResponse>> GetRecipe(string name)
    {
        var model = await _service.GetRecipe(name);
        var dto = _mapper.Map<RecipeDto>(model);
        return Ok(new GetRecipeResponse(dto));
    }

    [HttpGet("all")]
    [AllowAnonymous]
    public async Task<ActionResult<GetAllRecipesResponse>> GetAll([FromQuery,  Bind(Prefix = "")] GetAllRecipes query)
    {
        var (items, page, size, totalPages) = await _service.GetAllRecipes(query);

        var dtos = items.Select(r => _mapper.Map<RecipeDto>(r)).ToList();
        return Ok(new GetAllRecipesResponse(dtos, page, size, totalPages));
    }

    [HttpGet("search")]
    [AllowAnonymous]
    public async Task<ActionResult<GetAllRecipesResponse>> Search([FromQuery, Bind(Prefix = "")] SearchRecipes query)
    {
        var (items, page, size, totalPages) = await _service.SearchRecipes(query);
        var dtos = items.Select(r => _mapper.Map<RecipeDto>(r)).ToList();

        return Ok(new GetAllRecipesResponse(dtos, page, size, totalPages));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrPutRecipeRequest request)
    {
        var model = _mapper.Map<RecipeModel>(request);
        await _service.CreateRecipe(model);

        return StatusCode(StatusCodes.Status201Created);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] CreateOrPutRecipeRequest request)
    {
        /* meestal wil je een put nog steeds binden met een unique key maar de assesment had specifiek gevraagd voor
        home route. dus ik heb het zo gelaten. Na dit punt in de code heb ik gewerkt met het idee dat het origineel /{name} is.
        */
        var model = _mapper.Map<RecipeModel>(request);
        await _service.UpdateRecipe(model.Name, model);

        return NoContent();
    }

    [HttpPatch("{name}")]
    public async Task<IActionResult> Patch(string name, [FromBody] PatchRecipeRequest request)
    {
        var patch = new PatchRecipeCommand
        {
            Ingredients = request.Ingredients,
            Instructions = request.Instructions
        };

        await _service.PatchRecipe(name, patch);
        return NoContent();
    }

    [HttpDelete("{name}")]
    public async Task<IActionResult> Delete(string name)
    {
        await _service.DeleteAsync(name);
        return NoContent();
    }
}