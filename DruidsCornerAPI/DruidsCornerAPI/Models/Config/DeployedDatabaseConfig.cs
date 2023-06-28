using Microsoft.Extensions.Primitives;

namespace DruidsCornerAPI.Models.Config
{
    public record DeployedDatabaseConfig
    {
        public static readonly string SectionName = "DeployedDatabaseConfig";

        ///<summary>
        /// Environment variable that can be used in the configuration json files in order to set the DeployedDB location (among other things
        /// It is used to have a platform-agnostic way of injecting a deployed database from the deployment itself.
        ///</summary>
        protected static readonly string DeployedDBEnvVarName = "DRUIDSCORNERAPI_DIR";

        /// <summary>
        /// Encodes the location of RootFolderPath in the locally deployed file-based database
        /// </summary>
        public string RootFolderPath { get; set; } = string.Empty;

        /// <summary>
        /// Encodes the location of the Images folder, in the deployed context environment
        /// </summary>
        public string ImagesFolderPath { get; set; } = string.Empty;

        /// <summary>
        /// Encodes the location of the Pdf pages folder, in the deployed context environment
        /// </summary>
        public string PdfPagesFolderPath { get; set; } = string.Empty;

        /// <summary>
        /// Encodes the location of the Recipes folder, in the deployed context environment
        /// </summary>
        public string RecipesFolderPath { get; set; } = string.Empty;


        /// <summary>
        /// Tries to read members from the "DeployedDatabaseConfig" section
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        public bool FromConfigSection(IConfigurationSection section)
        {
            bool success = true;
            var rootFolderPath = section.GetValue<string>(nameof(RootFolderPath));
            var pdfPagesFolderPath = section.GetValue<string>(nameof(PdfPagesFolderPath));
            var imagesFolderPath = section.GetValue<string>(nameof(ImagesFolderPath));
            var recipesFolderPath = section.GetValue<string>(nameof(RecipesFolderPath));

            RootFolderPath = rootFolderPath ?? string.Empty;
            PdfPagesFolderPath = pdfPagesFolderPath?? string.Empty;
            ImagesFolderPath = imagesFolderPath ?? string.Empty;
            RecipesFolderPath = recipesFolderPath ?? string.Empty;

            // String substitution with env variable value
            string? deployedEnvVarValue = Environment.GetEnvironmentVariable(DeployedDBEnvVarName);
            var envVarPattern = $"${{{DeployedDBEnvVarName}}}";
            if (Path.Exists(deployedEnvVarValue))
            {
                RootFolderPath = RootFolderPath.Replace(envVarPattern, deployedEnvVarValue);
                PdfPagesFolderPath = PdfPagesFolderPath.Replace(envVarPattern, deployedEnvVarValue);
                ImagesFolderPath = ImagesFolderPath.Replace(envVarPattern, deployedEnvVarValue);
                RecipesFolderPath = RecipesFolderPath.Replace(envVarPattern, deployedEnvVarValue);
            }

            // Path.Exists() already takes care about potentially null-values as well.
            success &= Path.Exists(RootFolderPath);
            success &= Path.Exists(PdfPagesFolderPath);
            success &= Path.Exists(ImagesFolderPath);
            success &= Path.Exists(RecipesFolderPath);

            return success;
        }

        /// <summary>
        /// Tries to read from the configuration file itself
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public bool FromConfig(IConfiguration config)
        {
            var section = config.GetSection(SectionName);
            if (section == null)
            {
                return false;
            }
            return FromConfigSection(section);
        }
    }
}
