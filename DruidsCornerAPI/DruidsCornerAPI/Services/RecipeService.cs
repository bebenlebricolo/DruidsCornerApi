using DruidsCornerAPI.DatabaseHandlers;
using DruidsCornerAPI.Models.Config;
using DruidsCornerAPI.Models.Search;
using DruidsCornerAPI.Models.DiyDog.RecipeDb;

namespace DruidsCornerAPI.Services
{

    /// <summary>
    /// Recipe service class ; provides database services abstractions to Controller (and various environment-based behaviours)
    /// </summary>
    public class RecipeService
    {
        private readonly IConfiguration _configuration;
        private ILogger<RecipeService> _logger;
        private IDatabaseHandler? _dbHandler;

        /// <summary>
        /// Standard constructor
        /// </summary>
        /// <param name="configuration">Base configuration for the webapi</param>
        /// <param name="logger">Logger for this service</param>
        /// <param name="databaseHandler">Database handler. Left null, it'll trigger this service to retrieve the right version based on the configuration</param>
        public RecipeService(IConfiguration configuration, ILogger<RecipeService> logger, IDatabaseHandler? databaseHandler = null)
        {
            _configuration = configuration;
            _logger = logger;
            _dbHandler = databaseHandler;
        }

        private IDatabaseHandler GetDatabaseHandler()
        {
            if(_dbHandler == null)
            {
                _dbHandler = DatabaseHandlerFactory.GetDatabaseHandler(_configuration); 
            }
            return _dbHandler;
        }

        /// <summary>
        /// Fetches all recipes from database provider
        /// </summary>
        /// <returns></returns>
        public async Task<List<Recipe>?> GetAllRecipesAsync()
        {
            return await GetDatabaseHandler().GetAllRecipesAsync();
        }

        /// <summary>
        /// Fetches a single recipe by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<RecipeResult?> GetRecipeByNameAsync(string name)
        {
            return await GetDatabaseHandler().GetRecipeByNameAsync(name);
        }


        /// <summary>
        /// Fetches a single recipe by number
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public async Task<Recipe?> GetRecipeByNumberAsync(uint number)
        {
            return await GetDatabaseHandler().GetRecipeByNumberAsync(number);
        }

        /// <summary>
        /// Fetches a single recipe image using its number/id as a query parameter
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public async Task<Stream?> GetRecipeImageAsync(uint number)
        {
            return await GetDatabaseHandler().GetRecipeImageAsync(number);
        }

        /// <summary>
        /// Fetches a single recipe PDF page using its number/id as a query parameter
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public async Task<Stream?> GetRecipePdfPageAsync(uint number)
        {
            return await GetDatabaseHandler().GetRecipePdfPageAsync(number);
        }
    }
}
