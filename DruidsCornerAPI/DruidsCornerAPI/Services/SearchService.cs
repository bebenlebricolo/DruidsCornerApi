using DruidsCornerAPI.DatabaseHandlers;
using DruidsCornerAPI.Models.Search;
using DruidsCornerAPI.Models.DiyDog.RecipeDb;

namespace DruidsCornerAPI.Services
{

    /// <summary>
    /// Search Service class, allows to dig in some database content and use custom filters
    /// To retrieve Recipes from it.
    /// </summary>
    public class SearchService
    {
        private readonly IConfiguration _configuration;
        private ILogger<SearchService> _logger;

        /// <summary>
        /// Standard constructor
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public SearchService(IConfiguration configuration, ILogger<SearchService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

    
        /// <summary>
        /// Searches for a single recipe using the given queries and database handler.
        /// </summary>
        /// <param name="queries">Input queries used for searching</param>
        /// <param name="dbHandler">Database handler used in current context</param>
        /// <returns></returns>
        public async Task<RecipeResult> SearchRecipe(Queries queries, IDatabaseHandler dbHandler)
        {
            var output = new RecipeResult(0, new Recipe());
            return output;
        }
    }
}
