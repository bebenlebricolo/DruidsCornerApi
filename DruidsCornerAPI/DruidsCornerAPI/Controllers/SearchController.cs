using DruidsCornerAPI.Models.DiyDog.RecipeDb;
using DruidsCornerAPI.Services;
using DruidsCornerAPI.Models.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace DruidsCornerAPI.Controllers
{
    /// <summary>
    /// Operates on recipes.
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("search")]
    [ProducesResponseType(200)]
    // 300 is produced whenever accessing the http endpoint -> redirects to https adress with 302 status code (automatically done by GCP)
    [ProducesResponseType(302)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]

    public class SearchController : ControllerBase
    {
        private readonly ILogger<SearchController> _logger;
        private readonly RecipeService _recipeService;

        /// <summary>
        /// Standard constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="recipeService"></param>
        public SearchController(ILogger<SearchController> logger, RecipeService recipeService)
        {
            _logger = logger;
            _recipeService = recipeService;
        }

        /// <summary>
        /// Retrieves all available recipes from database
        /// </summary>
        /// <returns>Recipe list, or NotFound error</returns>
        [HttpGet("all", Name = "List all matching recipes")]
        [ProducesResponseType(typeof(MultipleRecipeResult), 200)]
        public async Task<IActionResult> SearchAllWithMatch()
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
    }
}