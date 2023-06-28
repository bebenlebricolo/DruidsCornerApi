using DruidsCornerAPI.DatabaseHandlers;
using DruidsCornerAPI.Models.Config;
using DruidsCornerAPI.Models.DiyDog;
using DruidsCornerAPI.Models.Exceptions;

namespace DruidsCornerAPI.Services
{
    /// <summary>
    /// Selects where the database should be retrieved from.
    /// </summary>
    public enum DatabaseSourceMode
    {
        Local, // Database will be retrieved locally (using the appsettings.json configuration)
        Cloud, // Database will be retrieved from Cloud Storage
        Unknown    // Default, used to encode the "Failure" mode, when input data parsing is unsuccessful.
    }

    public class RecipeService
    {
        private readonly IConfiguration _configuration;
        private ILogger<RecipeService> _logger;
        private static string DBModeEnvVarName = "DRUIDSCORNERAPI_DBMODE";

        public RecipeService(IConfiguration configuration, ILogger<RecipeService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        private DeployedDatabaseConfig GetDeployedDataPathFromConfig()
        {
            var deployedDb = new DeployedDatabaseConfig();
            var success = deployedDb.FromConfig(_configuration);
            if (!success)
            {
                _logger.LogError("Could not retrieve local file database from configuration !");
            }
            return deployedDb;
        }

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
        public async Task<Recipe?> GetRecipeByNameAsync(string name)
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

        public async Task<Stream?> GetRecipeImageAsync(uint number)
        {
            var dbMode = GetMode();
            var dbHandler = GetDatabaseHandler(dbMode);

            return await dbHandler.GetRecipeImageAsync(number);
        }

        public async Task<Stream?> GetRecipePdfPageAsync(uint number)
        {
            var dbMode = GetMode();
            var dbHandler = GetDatabaseHandler(dbMode);

            return await dbHandler.GetRecipePdfPageAsync(number);
        }
    }
}
