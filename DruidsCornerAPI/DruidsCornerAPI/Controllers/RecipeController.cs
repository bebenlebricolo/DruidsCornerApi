using DruidsCornerAPI.Models.DiyDog;
using DruidsCornerAPI.Services;
using DruidsCornerAPI.Models.SearchResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace DruidsCornerAPI.Controllers
{
    /// <summary>
    /// Operates on recipes.
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("recipe")]
    [ProducesResponseType(200)]
    // 300 is produced whenever accessing the http endpoint -> redirects to https adress with 302 status code (automatically done by GCP)
    [ProducesResponseType(302)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]

    public class RecipeController : ControllerBase
    {
        private readonly ILogger<RecipeController> _logger;
        private readonly RecipeService _recipeService;

        /// <summary>
        /// Standard constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="recipeService"></param>
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
        /// Fuzzy search is used along the way, so if a 100% matching key is not provided, the algorithm will try
        /// to find the closest matching recipe that has the requested name.
        /// An optional "probability" parameter can be fed to the endpoint to remove any candidate whose hit probability is lower than
        /// a certain threshold.
        /// </summary>
        ///<param name="name">Name of the selected recipe.</param>
        ///<param name="probabilityThreshold">Optional probability threshold (aka "Score"). Acceptable range (-1,100).
        /// If this parameter is set, recipes will be filtered with this threshold as the minimum acceptable probability threshold
        /// for candidates recipes. Otherwise (left empty), no filter will be applied on the results. </param>
        /// <returns>Found recipe, or NotFound error</returns>
        [HttpGet("byname", Name = "Get a single recipe by name")]
        [ProducesResponseType(typeof(RecipeResult), 200)]
        public async Task<IActionResult> FetchSingle([FromQuery] string name, [FromQuery] int probabilityThreshold = -1)
        {
            try
            {
                // Clamp input data, we never know what end-users can feed in as values !
                probabilityThreshold = Math.Clamp(probabilityThreshold, -1, 100);
                
                var recipe = await _recipeService.GetRecipeByNameAsync(name);
                if (probabilityThreshold != -1 && recipe.Probability < probabilityThreshold)
                {
                    return NotFound($"Could not find the requested recipe with query : {name}");
                }
                return Ok(recipe);
            }
            catch(Exception ex)
            {
                _logger.LogError($"Caught error while retrieving recipe by id : {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}