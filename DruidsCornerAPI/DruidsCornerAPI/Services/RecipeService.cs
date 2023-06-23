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
        LocalMode, // Database will be retrieved locally (using the appsettings.json configuration)
        CloudMode, // Database will be retrieved from Cloud Storage
        Unknown    // Default, used to encode the "Failure" mode, when input data parsing is unsuccessful.
    }

    public class RecipeService
    {
        private readonly IConfiguration _configuration;
        private ILogger<RecipeService> _logger;
        private static string DBModeEnvVarName = "DBMode";

        public RecipeService(IConfiguration configuration, Logger<RecipeService> logger)
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


        public async Task<List<Recipe>> GetAllRecipesAsync()
        {
            var dbModeEnum = DatabaseSourceMode.LocalMode;
            var dbModeEnv = Environment.GetEnvironmentVariable(DBModeEnvVarName);
            if(false == Enum.TryParse(dbModeEnv, out dbModeEnum))
            {
                // Enforce the LocalMode anyway
                dbModeEnum = DatabaseSourceMode.LocalMode;
            }

            // Local deployment needs proper path handling
            if(dbModeEnum == DatabaseSourceMode.LocalMode)
            {
                var deployedConfig = new DeployedDatabaseConfig();
                var parsingSuccess = deployedConfig.FromConfig(_configuration);
                if(!parsingSuccess)
                {
                    throw new ConfigException("Could not read configuration (appsettings), failing short.");
                }

                var dbHandler = new LocalDatabaseHandler(deployedConfig);
                return await dbHandler.GetAllRecipeAsync();
            }
            else
            {
                throw new NotImplementedException("Whoops ! Not there yet!");
            }
        }


    }
}
