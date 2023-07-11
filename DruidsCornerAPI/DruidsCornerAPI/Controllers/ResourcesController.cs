using DruidsCornerAPI.Models.DiyDog;
using DruidsCornerAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DruidsCornerAPI.Controllers
{
    /// <summary>
    /// Operates on recipes.
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("resources")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public class ResourcesController : ControllerBase
    {
        private readonly ILogger<ResourcesController> _logger;
        private readonly RecipeService _recipeService;

        /// <summary>
        /// Standard constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="recipeService"></param>
        public ResourcesController(ILogger<ResourcesController> logger, RecipeService recipeService)
        {
            _logger = logger;
            _recipeService = recipeService;
        }

        /// <summary>
        /// Fetches a beer image (PNG) using its id.
        /// </summary>
        /// <param name="number">Beer's index, numeral form.</param>
        /// <returns>
        ///     Image source (buffered stream), or NotFoundError(404)
        /// </returns>
        [HttpGet("image", Name = "Fetches beer's image")]
        [ProducesResponseType(500)]
        [Produces("application/octet-stream")]
        [ProducesResponseType(typeof(FileStreamResult), 200)]
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
                return NotFound("Could not fetch requested image");
            }
            catch (Exception ex) 
            {
                _logger.LogError($"Caught error while access file {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        /// <summary>
        /// Fetches a single beer's pdf page
        /// </summary>
        /// <param name="number">Beer's id, numeral form.</param>
        /// <returns>
        ///     Pdf page source, buffered stream or NotFoundError(404)
        /// </returns>
        [HttpGet("pdf", Name = "Fetches beer's pdf page")]
        [Produces("application/octet-stream")]
        [ProducesResponseType(typeof(FileStreamResult), 200)]
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
                return NotFound("Could not fetch requested pdf.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Caught error while access file {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}