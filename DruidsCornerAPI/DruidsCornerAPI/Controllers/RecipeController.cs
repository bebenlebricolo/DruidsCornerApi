using DruidsCornerAPI.Models;
using DruidsCornerAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace DruidsCornerAPI.Controllers
{
    [ApiController]
    [Route("recipe")]
    //[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
    public class RecipeController : ControllerBase
    {
        private readonly ILogger<RecipeController> _logger;
        private RecipeService _recipeService;

        public RecipeController(ILogger<RecipeController> logger, RecipeService recipeService)
        {
            _logger = logger;
            _recipeService = recipeService;
        }

        [Authorize]
        [HttpGet(Name = "ListAllRecipes")]
        public async Task<IActionResult> ListAllRecipes()
        {
            var allRecipes = await _recipeService.GetAllRecipesAsync();
            if(allRecipes.Count != 0)
            {
                return Ok(allRecipes);
            }
            return StatusCode(500, "Internal Server Error");
        }

    }
}