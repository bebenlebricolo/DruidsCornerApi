using DruidsCornerAPI.Models.DiyDog;
using DruidsCornerAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FuzzySharp;

namespace DruidsCornerAPI.Controllers
{
    /// <summary>
    /// Operates on recipes.
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("recipe")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]

    public class RecipeController : ControllerBase
    {
        private readonly ILogger<RecipeController> _logger;
        private readonly RecipeService _recipeService;

        public RecipeController(ILogger<RecipeController> logger, RecipeService recipeService)
        {
            _logger = logger;
            _recipeService = recipeService;
        }

        /// <summary>
        /// Retrieves all available recipes from database
        /// </summary>
        /// <returns>Recipe list, or NotFound error</returns>
        [HttpGet("all", Name = "ListAllRecipes")]
        [ProducesResponseType(typeof(AllRecipesCollection), 200)]
        public async Task<IActionResult> ListAllRecipes()
        {
            try
            {
                var allRecipes = await _recipeService.GetAllRecipesAsync();
                if (allRecipes.Count != 0)
                {
                    return Ok(allRecipes);
                }
                return NotFound("Could not retrieve recipes");
            }
            catch(Exception ex)
            {
                _logger.LogError($"Caught error while retrieving recipe by id : {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        /// <summary>
        /// Retrieves a single recipe, using its id as a search key.
        /// </summary>
        /// <param name="number">Id of the recipe, in a numeral form.</param>
        /// <returns>Recipe, or NotFound error</returns>
        [HttpGet("bynumber", Name = "Get a single recipe by id")]
        [ProducesResponseType(typeof(Recipe), 200)]
        public async Task<IActionResult> FetchSingle([FromQuery] uint number)
        {
            try
            {
                var recipe = await _recipeService.GetRecipeByNumberAsync(number);
                if (recipe != null)
                {
                    return Ok(recipe);
                }
                else
                {
                    return NotFound("Could not find targeted recipe");
                }
            }
            catch(Exception ex)
            {
                _logger.LogError($"Caught error while retrieving recipe by id : {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }
        
        /// <summary>
        /// Retrieves a single recipe using its name as a search key.
        /// </summary>
        ///<param name="name">Name of the selected recipe.</param>
        /// <returns>Recipe list, or NotFound error</returns>
        [HttpGet("byname", Name = "Get a single recipe by name")]
        [ProducesResponseType(typeof(Recipe), 200)]
        public async Task<IActionResult> FetchSingle([FromQuery] string name)
        {
            try
            {
                var recipe = await _recipeService.GetRecipeByNameAsync(name);
                if (recipe != null)
                {
                    return Ok(recipe);
                }
                return NotFound("Could not retrieve targeted recipe.");
            }
            catch(Exception ex)
            {
                _logger.LogError($"Caught error while retrieving recipe by id : {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}