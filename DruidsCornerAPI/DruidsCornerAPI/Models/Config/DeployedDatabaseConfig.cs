using Microsoft.Extensions.Primitives;
using System.IO;

namespace DruidsCornerAPI.Models.Config
{
    /// <summary>
    /// Models out the locally deployed database configuration (used in an appsettings.json)
    /// </summary>
    /// <value></value>
    public record DeployedDatabaseConfig
    {
        /// <summary>
        /// Base section name for the local deployed Database Config stuff
        /// </summary>
        public static readonly string SectionName = "DeployedDatabaseConfig";

        ///<summary>
        /// Environment variable that can be used in the configuration json files in order to set the DeployedDB location (among other things
        /// It is used to have a platform-agnostic way of injecting a deployed database from the deployment itself.
        ///</summary>
        protected static readonly string DeployedDBEnvVarName = "DRUIDSCORNERAPI_DIR";

        /// <summary>
        /// Encodes the location of RootFolderPath in the locally deployed file-based database
        /// </summary>
        protected string RootFolderPath { get; private set;} = Path.GetTempPath();

        /// <summary>
        /// Encodes the location of the Images folder, in the deployed context environment
        /// </summary>
        protected string ImagesFolderName { get; set; } = "images";
        
        /// <summary>
        /// Encodes the location of the Pdf pages folder, in the deployed context environment
        /// </summary>
        protected string PdfPagesFolderName { get; set; } = "pdf_pages";


        /// <summary>
        /// Encodes the location of the Recipes folder, in the deployed context environment
        /// </summary>
        protected string RecipesFolderName { get; set; } = "recipes";


        /// <summary>
        /// Encodes the name of reversed indexed databases
        /// </summary>
        protected string IndexedDbFolderName { get; set; } = "dbanalysis";


        /// <summary>
        /// Tries to read members from the "DeployedDatabaseConfig" section
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        public bool FromConfigSection(IConfigurationSection section)
        {
            bool success = true;
            var rootFolderName = section.GetValue<string>(nameof(RootFolderPath));
            var pdfPagesFolderName = section.GetValue<string>(nameof(PdfPagesFolderName));
            var imagesFolderName = section.GetValue<string>(nameof(ImagesFolderName));
            var recipesFolderName = section.GetValue<string>(nameof(RecipesFolderName));
            var indexedDbFolderName = section.GetValue<string>(nameof(IndexedDbFolderName));

            RootFolderPath = rootFolderName ?? RootFolderPath;
            PdfPagesFolderName = pdfPagesFolderName ?? PdfPagesFolderName;
            ImagesFolderName = imagesFolderName ?? ImagesFolderName;
            RecipesFolderName = recipesFolderName ?? RecipesFolderName;
            IndexedDbFolderName = indexedDbFolderName ?? IndexedDbFolderName;

            // String substitution with env variable value
            string? deployedEnvVarValue = Environment.GetEnvironmentVariable(DeployedDBEnvVarName);
            var envVarPattern = $"${{{DeployedDBEnvVarName}}}";
            if(deployedEnvVarValue is not null)
            {
                RootFolderPath = RootFolderPath.Replace(envVarPattern, deployedEnvVarValue);
            }
            else 
            {
                throw new Exception($"Could not find {DeployedDBEnvVarName} environment variable in current context.");
            }

            // Path.Exists() already takes care about potentially null-values as well.
            success &= Path.Exists(RootFolderPath);
            
            success &= GetRootFolder().Exists;
            success &= GetImagesFolder().Exists;
            success &= GetRecipesFolder().Exists;
            success &= GetPdfPagesFolder().Exists;
            success &= GetIndexedDbFolder().Exists;

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

        public DirectoryInfo GetRootFolder()
        {
            return new DirectoryInfo(RootFolderPath);
        }
        
        
        public DirectoryInfo GetRecipesFolder()
        {
            return new DirectoryInfo(Path.Combine(RootFolderPath, RecipesFolderName));
        }

        public DirectoryInfo GetImagesFolder()
        {
            return new DirectoryInfo(Path.Combine(RootFolderPath, ImagesFolderName));
        }

        public DirectoryInfo GetPdfPagesFolder()
        {
            return new DirectoryInfo(Path.Combine(RootFolderPath, PdfPagesFolderName));
        }

        public DirectoryInfo GetIndexedDbFolder()
        {
            return new DirectoryInfo(Path.Combine(RootFolderPath, IndexedDbFolderName));
        }
    }
}
