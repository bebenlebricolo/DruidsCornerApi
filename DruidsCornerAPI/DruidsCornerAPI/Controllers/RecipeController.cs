using DruidsCornerAPI.Models;
using DruidsCornerAPI.Models.DiyDog;
using DruidsCornerAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace DruidsCornerAPI.Controllers
{
    /// <summary>
    /// Operates on recipes.
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("recipe")]
    public class RecipeController : ControllerBase
    {
        private readonly ILogger<RecipeController> _logger;
        private RecipeService _recipeService;

        public RecipeController(ILogger<RecipeController> logger, RecipeService recipeService)
        {
            _logger = logger;
            _recipeService = recipeService;
        }

        /// <summary>
        /// Retrieves all available recipes
        /// </summary>
        /// <returns></returns>
        [HttpGet(Name = "ListAllRecipes")]
        [ProducesResponseType(500)]
        [ProducesResponseType(typeof(AllRecipesCollection), 200)]

        public async Task<IActionResult> ListAllRecipes()
        {
            var allRecipes = await _recipeService.GetAllRecipesAsync();
            if(allRecipes.Count != 0)
            {
                return Ok(allRecipes);
            }
            return StatusCode(500, "Internal Server Error");
        }

        /// <summary>
        /// Retrieves all available recipes
        /// </summary>
        /// <returns></returns>
        [HttpGet("byname", Name = "Get a single recipe")]
        public async Task<IActionResult> FetchSingle([FromQuery] string name)
        {
            var recipe= await _recipeService.GetRecipeByNameAsync(name);
            if (recipe != null)
            {
                return Ok(recipe);
            }
            return StatusCode(500, "Internal Server Error");
        }

    }
}