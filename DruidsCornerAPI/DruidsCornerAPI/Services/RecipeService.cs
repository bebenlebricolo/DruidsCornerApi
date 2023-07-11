using DruidsCornerAPI.DatabaseHandlers;
using DruidsCornerAPI.Models.Config;
using DruidsCornerAPI.Models.SearchResults;
using DruidsCornerAPI.Models.DiyDog;
using DruidsCornerAPI.Models.Exceptions;

namespace DruidsCornerAPI.Services
{
    /// <summary>
    /// Selects where the database should be retrieved from.
    /// </summary>
    public enum DatabaseSourceMode
    {
        /// <summary> Database will be retrieved locally (using the appsettings.json configuration)  </summary>
        Local,
        
        /// <summary> Database will be retrieved from Cloud Storage </summary>
        Cloud,

        /// <summary> Default, used to encode the "Failure" mode, when input data parsing is unsuccessful. </summary>
        Unknown  
    }

    /// <summary>
    /// Recipe service class ; provides database services abstractions to Controller (and various environment-based behaviours)
    /// </summary>
    public class RecipeService
    {
        private readonly IConfiguration _configuration;
        private ILogger<RecipeService> _logger;
        private static string DBModeEnvVarName = "DRUIDSCORNERAPI_DBMODE";

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
        /// Retrieves Database mode from local environment
        /// </summary>
        /// <returns></returns>
        protected DatabaseSourceMode GetMode()
        {
            DatabaseSourceMode dbModeEnum;
           
            var dbModeEnv = Environment.GetEnvironmentVariable(DBModeEnvVarName);
            if (false == Enum.TryParse(dbModeEnv, out dbModeEnum))
            {
                // Enforce the LocalMode anyway
                dbModeEnum = DatabaseSourceMode.Local;
            }
            return dbModeEnum;
        }

        /// <summary>
        /// Builds / retrieves a database handler that suits the local DB environment (either Local or Cloud Based)
        /// </summary>
        /// <param name="dbMode"></param>
        /// <returns></returns>
        protected IDatabaseHandler GetDatabaseHandler(DatabaseSourceMode dbMode) 
        {
            // Local deployment needs proper path handling
            if (dbMode == DatabaseSourceMode.Local)
            {
                var deployedConfig = new DeployedDatabaseConfig();
                var parsingSuccess = deployedConfig.FromConfig(_configuration);
                if (!parsingSuccess)
                {
                    throw new ConfigException("Could not read configuration (appsettings), failing short.");
                }

                var dbHandler = new LocalDatabaseHandler(deployedConfig);
                return dbHandler;
            }
        
            throw new NotImplementedException("Whoops ! Not there yet!");
        }

        /// <summary>
        /// Fetches all recipes from database provider
        /// </summary>
        /// <returns></returns>
        public async Task<List<Recipe>> GetAllRecipesAsync()
        {
            var dbMode = GetMode();
            var dbHandler = GetDatabaseHandler(dbMode);

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
            var dbMode = GetMode();
            var dbHandler = GetDatabaseHandler(dbMode);

            return await dbHandler.GetRecipeByNameAsync(name);
        }


        /// <summary>
        /// Fetches a single recipe by number
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public async Task<Recipe?> GetRecipeByNumberAsync(uint number)
        {
            var dbMode = GetMode();
            var dbHandler = GetDatabaseHandler(dbMode);

            return await dbHandler.GetRecipeByNumberAsync(number);
        }

        /// <summary>
        /// Fetches a single recipe image using its number/id as a query parameter
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public async Task<Stream?> GetRecipeImageAsync(uint number)
        {
            var dbMode = GetMode();
            var dbHandler = GetDatabaseHandler(dbMode);

            return await dbHandler.GetRecipeImageAsync(number);
        }

        /// <summary>
        /// Fetches a single recipe PDF page using its number/id as a query parameter
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public async Task<Stream?> GetRecipePdfPageAsync(uint number)
        {
            var dbMode = GetMode();
            var dbHandler = GetDatabaseHandler(dbMode);

            return await dbHandler.GetRecipePdfPageAsync(number);
        }
    }
}
