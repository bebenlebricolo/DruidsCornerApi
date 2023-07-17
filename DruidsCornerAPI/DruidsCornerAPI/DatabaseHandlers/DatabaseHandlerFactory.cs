using DruidsCornerAPI.Models.Config;
using DruidsCornerAPI.Models.Exceptions;
using DruidsCornerAPI.Tools.Logging;

namespace DruidsCornerAPI.DatabaseHandlers
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
    /// Database Handler factory static class -> builds the right database handler depending on the selected source mode
    /// </summary>
    public static class DatabaseHandlerFactory
    {
        private static string DBModeEnvVarName = "DRUIDSCORNERAPI_DBMODE";
        
        /// <summary>
        /// Database configuration.
        /// </summary>
        public static DeployedDatabaseConfig? _dbConfig = null;

        /// <summary>
        /// Builds / retrieves a database handler that suits the local DB environment (either Local or Cloud Based)
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IDatabaseHandler GetDatabaseHandler(IConfiguration configuration) 
        {
            if(_dbConfig == null)
            {
                _dbConfig = new DeployedDatabaseConfig();
                _dbConfig.FromConfig(configuration);
            }
            var dbMode = GetMode();
            return GetDatabaseHandler(dbMode, _dbConfig);
        }

        /// <summary>
        /// Builds / retrieves a database handler that suits the local DB environment (either Local or Cloud Based)
        /// </summary>
        /// <param name="dbMode"></param>
        /// <param name="dbConfig"></param>
        /// <returns></returns>
        public static IDatabaseHandler GetDatabaseHandler(DatabaseSourceMode dbMode, DeployedDatabaseConfig dbConfig) 
        {
            // Local deployment needs proper path handling
            if (dbMode == DatabaseSourceMode.Local)
            {
                var dbLogger = ApplicationLogging.LoggerFactory.CreateLogger<LocalDatabaseHandler>();
                var dbHandler = new LocalDatabaseHandler(dbConfig, dbLogger);
                return dbHandler;
            }
        
            throw new NotImplementedException("Whoops ! Not there yet!");
        }

        /// <summary>
        /// Retrieves Database mode from local environment
        /// </summary>
        /// <returns></returns>
        public static DatabaseSourceMode GetMode()
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
    }
}