using DruidsCornerAPI.Models;
using DruidsCornerAPI.Models.DiyDog;
using DruidsCornerAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;

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
            if (allRecipes.Count != 0)
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
            var recipe = await _recipeService.GetRecipeByNameAsync(name);
            if (recipe != null)
            {
                return Ok(recipe);
            }
            return StatusCode(500, "Internal Server Error");
        }

        /// <summary>
        /// Retrieves all available recipes
        /// </summary>
        /// <returns></returns>
        [HttpGet("image", Name = "Fetches beer's image")]
        [ProducesResponseType(500)]
        [Produces("application/octet-stream")]
        public async Task<IActionResult> FetchSingleImage([FromQuery] uint number)
        {
            try
            {
                var stream = await _recipeService.GetRecipeImageAsync(number);
                if (stream != null)
                {
                    var result = File(stream, "image/png", "image.png");
                    
                    return result;
                }
                return StatusCode(500, "Internal Server Error");
            }
            catch (Exception ex) 
            {
                _logger.LogError($"Caught error while access file {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        /// <summary>
        /// Retrieves all available recipes
        /// </summary>
        /// <returns></returns>
        [HttpGet("pdf", Name = "Fetches beer's pdf page")]
        [ProducesResponseType(500)]
        [Produces("application/octet-stream")]
        public async Task<IActionResult> FetchSinglePdf([FromQuery] uint number)
        {
            try
            {
                var stream = await _recipeService.GetRecipePdfPageAsync(number);
                if (stream != null)
                {
                    var result = File(stream, "application/pdf", "beer.pdf");
                    return result;
                }
                return StatusCode(500, "Internal Server Error");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Caught error while access file {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}