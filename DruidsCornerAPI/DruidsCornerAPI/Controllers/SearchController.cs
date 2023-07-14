using DruidsCornerAPI.Models.DiyDog.RecipeDb;
using DruidsCornerAPI.Services;
using DruidsCornerAPI.Models.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DruidsCornerAPI.Models.Config;
using DruidsCornerAPI.Models.DiyDog.References;

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
        private readonly IConfiguration _configuration;
        private readonly ILogger<SearchController> _logger;
        private readonly RecipeService _recipeService;
        private readonly SearchService _searchService;

        private readonly DeployedDatabaseConfig _dbConfig;

        /// <summary>
        /// Standard constructor
        /// </summary>
        /// <param name="configuration">Application configuration</param>
        /// <param name="logger">System logger for this class</param>
        /// <param name="recipeService">Recipe service object</param>
        /// <param name="searchService">Search service object</param>
        public SearchController(IConfiguration configuration,
                                ILogger<SearchController> logger,
                                RecipeService recipeService,
                                SearchService searchService)
        {
            _configuration = configuration;
            _logger = logger;
            _recipeService = recipeService;
            _searchService = searchService;
            _dbConfig = new DeployedDatabaseConfig();
        }

        /// <summary>
        /// Retrieves all available recipes from database
        /// </summary>
        /// <returns>Recipe list, or NotFound error</returns>
        [HttpPost("all", Name = "List all matching recipes")]
        [ProducesResponseType(typeof(List<Recipe>), 200)]
        public async Task<IActionResult> SearchAllWithMatch([FromBody] Queries queries)
        {
            try
            {
                var dbHandler = DatabaseHandlers.DatabaseHandlerFactory.GetDatabaseHandler(_configuration);
                var candidates = await _searchService.SearchRecipeAsync(queries, dbHandler);
                if(candidates.Count == 0)
                {
                    return NotFound("No Recipe match the input query.");
                }
                return Ok(candidates);

            }
            catch(Exception ex)
            {
                _logger.LogError($"Caught error while retrieving Hop with fuzzy search : {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        /// <summary>
        /// Retrieves all hops that match the input query
        /// </summary>
        /// <returns>Recipe list, or NotFound error</returns>
        [HttpGet("hops", Name = "Find hops that match input request")]
        [ProducesResponseType(typeof(List<HopProperty>), 200)]
        public async Task<IActionResult> SearchHopsWithMatch([FromQuery] List<string> names)
        {
            try
            {
                var dbHandler = DatabaseHandlers.DatabaseHandlerFactory.GetDatabaseHandler(_configuration);
                var refProp = await _searchService.SearchHopsWithQuery(names, dbHandler);
                if(refProp.Count == 0)
                {
                    return NotFound("No Hops match the input query.");
                }
                return Ok(refProp);

            }
            catch(Exception ex)
            {
                _logger.LogError($"Caught error while retrieving Hop with fuzzy search : {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        /// <summary>
        /// Retrieves all malts that match the input query
        /// </summary>
        /// <returns>Recipe list, or NotFound error</returns>
        [HttpGet("malts", Name = "Find malts that match input request")]
        [ProducesResponseType(typeof(List<MaltProperty>), 200)]
        public async Task<IActionResult> SearchMaltsWithMatch([FromQuery] List<string> names)
        {
            try
            {
                var dbHandler = DatabaseHandlers.DatabaseHandlerFactory.GetDatabaseHandler(_configuration);
                var refProp = await _searchService.SearchMaltsWithQuery(names, dbHandler);
                if(refProp.Count == 0)
                {
                    return NotFound("No Malt match the input query.");
                }
                return Ok(refProp);

            }
            catch(Exception ex)
            {
                _logger.LogError($"Caught error while retrieving Malt with fuzzy search : {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        /// <summary>
        /// Retrieves all yeasts that match the input query
        /// </summary>
        /// <returns>Recipe list, or NotFound error</returns>
        [HttpGet("yeasts", Name = "Find yeasts that match input request")]
        [ProducesResponseType(typeof(List<YeastProperty>), 200)]
        public async Task<IActionResult> SearchYeastsWithMatch([FromQuery] List<string> names)
        {
            try
            {
                var dbHandler = DatabaseHandlers.DatabaseHandlerFactory.GetDatabaseHandler(_configuration);
                var refProp = await _searchService.SearchYeastsWithQuery(names, dbHandler);
                if(refProp.Count == 0)
                {
                    return NotFound("No Yeast match the input query.");
                }
                return Ok(refProp);

            }
            catch(Exception ex)
            {
                _logger.LogError($"Caught error while retrieving Yeast with fuzzy search : {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        /// <summary>
        /// Retrieves all styles that match the input query
        /// </summary>
        /// <returns>Recipe list, or NotFound error</returns>
        [HttpGet("styles", Name = "Find styles that match input request")]
        [ProducesResponseType(typeof(List<StyleProperty>), 200)]
        public async Task<IActionResult> SearchStylesWithMatch([FromQuery] List<string> names,
                                                               [FromQuery] uint minimumMatchScore = 50)
        {
            try
            {
                minimumMatchScore = Math.Clamp(minimumMatchScore, 0, 100);
                var dbHandler = DatabaseHandlers.DatabaseHandlerFactory.GetDatabaseHandler(_configuration);
                var refProp = await _searchService.SearchStylesWithQuery(names, dbHandler, minimumMatchScore);
                if(refProp.Count == 0)
                {
                    return NotFound("No Style match the input query.");
                }
                return Ok(refProp);

            }
            catch(Exception ex)
            {
                _logger.LogError($"Caught error while retrieving Style with fuzzy search : {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}