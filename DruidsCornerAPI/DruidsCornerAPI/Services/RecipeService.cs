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

        /// <summary>
        /// Standard constructor
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public RecipeService(IConfiguration configuration, ILogger<RecipeService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }


        /// <summary>
        /// Fetches all recipes from database provider
        /// </summary>
        /// <returns></returns>
        public async Task<List<Recipe>> GetAllRecipesAsync()
        {
            var dbHandler = DatabaseHandlerFactory.GetDatabaseHandler(_configuration);
            return await dbHandler.GetAllRecipesAsync();
        }

        /// <summary>
        /// Fetches a single recipe by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<RecipeResult> GetRecipeByNameAsync(string name)
        {
            var dbHandler = DatabaseHandlerFactory.GetDatabaseHandler(_configuration);
            return await dbHandler.GetRecipeByNameAsync(name);
        }


        /// <summary>
        /// Fetches a single recipe by number
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public async Task<Recipe?> GetRecipeByNumberAsync(uint number)
        {
            var dbHandler = DatabaseHandlerFactory.GetDatabaseHandler(_configuration);
            return await dbHandler.GetRecipeByNumberAsync(number);
        }

        /// <summary>
        /// Fetches a single recipe image using its number/id as a query parameter
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public async Task<Stream?> GetRecipeImageAsync(uint number)
        {
            var dbHandler = DatabaseHandlerFactory.GetDatabaseHandler(_configuration);
            return await dbHandler.GetRecipeImageAsync(number);
        }

        /// <summary>
        /// Fetches a single recipe PDF page using its number/id as a query parameter
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public async Task<Stream?> GetRecipePdfPageAsync(uint number)
        {
            var dbHandler = DatabaseHandlerFactory.GetDatabaseHandler(_configuration);
            return await dbHandler.GetRecipePdfPageAsync(number);
        }
    }
}
