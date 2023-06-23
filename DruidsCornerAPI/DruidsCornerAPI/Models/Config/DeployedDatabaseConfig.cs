using Microsoft.Extensions.Primitives;

namespace DruidsCornerAPI.Models.Config
{
    public record DeployedDatabaseConfig 
    {
        public static readonly string SectionName = "DeployedDatabaseConfig";
    
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

            success &= rootFolderPath != null;
            success &= pdfPagesFolderPath!= null;
            success &= imagesFolderPath != null;
            success &= recipesFolderPath != null;

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
