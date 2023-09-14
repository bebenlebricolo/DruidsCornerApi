using System.Runtime.CompilerServices;
using DruidsCornerAPI.Models.Exceptions;

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
        public string RootFolderPath { get; private set;} = Path.GetTempPath();

        /// <summary>
        /// Encodes the location of the Images folder, in the deployed context environment
        /// </summary>
        public string ImagesFolderName { get; set; } = "images";
        
        /// <summary>
        /// Encodes the location of the Pdf pages folder, in the deployed context environment
        /// </summary>
        public string PdfPagesFolderName { get; set; } = "pdf_pages";


        /// <summary>
        /// Encodes the location of the Recipes folder, in the deployed context environment
        /// </summary>
        public string RecipesFolderName { get; set; } = "recipes";


        /// <summary>
        /// Encodes the name of reversed indexed databases
        /// </summary>
        public string IndexedDbFolderName { get; set; } = "dbanalysis";
        
        /// <summary>
        /// Encodes the name of the references file databases
        /// </summary>
        public string ReferencesFolderName { get; set; } = "references";

        /// <summary>
        /// Flags that states whether this configuration object was successfully configured previously or not7
        /// It is used to prevent multiple re-initialization when it's already initialized/configured appropriately
        /// Moreover, it allows to skip reading from file system when it's content is already stored in RAM.
        /// This flag is set by the <see cref="FromConfig"/> methods family.
        /// </summary>
        public bool Configured {get; private set;} = false;
        

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            var success = true;
            // Path.Exists() already takes care about potentially null-values as well.
            success &= Path.Exists(RootFolderPath);
            
            success &= GetRootFolder().Exists;
            success &= GetImagesFolder().Exists;
            success &= GetRecipesFolder().Exists;
            success &= GetPdfPagesFolder().Exists;
            success &= GetIndexedDbFolder().Exists;
            success &= GetReferencesFolder().Exists;

            return success;
        }

        /// <summary>
        /// Used as a standalone way of retrieving a deployed database config from local directory structure.
        /// Essentially used in tests
        /// </summary>
        /// <param name="rootFolderPath"></param>
        /// <returns></returns>
        public bool FromRootFolder(string rootFolderPath)
        {
            if(!Path.Exists(RootFolderPath))
            {
                return false;
            }

            RootFolderPath = rootFolderPath;
            var success = IsValid();
            if(success)
            {
                // Set the flag to allow upper layer code to skip re-initialization steps
                Configured = true;
            }
            return success;
        }

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
            var referencesFolderName = section.GetValue<string>(nameof(ReferencesFolderName));

            RootFolderPath = rootFolderName ?? RootFolderPath;
            PdfPagesFolderName = pdfPagesFolderName ?? PdfPagesFolderName;
            ImagesFolderName = imagesFolderName ?? ImagesFolderName;
            RecipesFolderName = recipesFolderName ?? RecipesFolderName;
            IndexedDbFolderName = indexedDbFolderName ?? IndexedDbFolderName;
            ReferencesFolderName = referencesFolderName ?? ReferencesFolderName;

            // String substitution with env variable value
            string? deployedEnvVarValue = Environment.GetEnvironmentVariable(DeployedDBEnvVarName);
            var envVarPattern = $"${{{DeployedDBEnvVarName}}}";
            if (RootFolderPath.Contains(envVarPattern))
            {
                if (deployedEnvVarValue != null)
                {
                    RootFolderPath = RootFolderPath.Replace(envVarPattern, deployedEnvVarValue);
                }
                else
                {
                    // That's not supposed to happen
                    throw new
                        ConfigException($"The environment variable {DeployedDBEnvVarName} was referenced in the config section but environment does not contain any value for it.'");
                }
            }
            

            success = IsValid();   
            if(success)
            {
                // Set the flag to allow upper layer code to skip re-initialization steps
                Configured = true;
            }
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

        /// <summary>
        /// Fetches the root folder of the local file database
        /// </summary>
        public DirectoryInfo GetRootFolder()
        {
            return new DirectoryInfo(RootFolderPath);
        }
        
        
        /// <summary>
        /// Retrieves the Recipes folder for the locally deployed database
        /// </summary>
        /// <returns>DirectoryInfo for the requested path</returns>
        public DirectoryInfo GetRecipesFolder()
        {
            return new DirectoryInfo(Path.Combine(RootFolderPath, RecipesFolderName));
        }

        /// <summary>
        /// Fetches the image folder for the locally deployed database
        /// </summary>
        /// <returns>DirectoryInfo for the requested path</returns>
        public DirectoryInfo GetImagesFolder()
        {
            return new DirectoryInfo(Path.Combine(RootFolderPath, ImagesFolderName));
        }

        /// <summary>
        /// Retrieves the PDF pages folder for the local deployed database.
        /// </summary>
        /// <returns>DirectoryInfo for the requested path</returns>
        public DirectoryInfo GetPdfPagesFolder()
        {
            return new DirectoryInfo(Path.Combine(RootFolderPath, PdfPagesFolderName));
        }

        /// <summary>
        /// Retrieves the indexed database folder for the locally deployed database
        /// </summary>
        /// <returns></returns>
        public DirectoryInfo GetIndexedDbFolder()
        {
            return new DirectoryInfo(Path.Combine(RootFolderPath, IndexedDbFolderName));
        }

        /// <summary>
        /// Retrieves the indexed database folder for the locally deployed database
        /// </summary>
        /// <returns></returns>
        public DirectoryInfo GetReferencesFolder()
        {
            return new DirectoryInfo(Path.Combine(RootFolderPath, ReferencesFolderName));
        }
    }
}
