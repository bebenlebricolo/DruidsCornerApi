using DruidsCornerAPI.Models.Config;
using DruidsCornerAPI.Models.DiyDog;
using DruidsCornerAPI.Tools;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DruidsCornerAPI.DatabaseHandlers
{
    public class LocalDatabaseHandler : IDatabaseHandler
    {
        private DirectoryInfo _deployedDBDir;
        private DirectoryInfo _imagesDir;
        private DirectoryInfo _pdfPagesDir;
        private DirectoryInfo _recipesDir;

        private List<Recipe> _cachedRecipes;
        private JsonSerializerOptions _jsonOptions;

        public LocalDatabaseHandler(DeployedDatabaseConfig config)
        {
            InitDirectories(config);
        }

        private void InitDirectories(DeployedDatabaseConfig config)
        {
            _deployedDBDir = new DirectoryInfo(config.RootFolderPath);
            _imagesDir = new DirectoryInfo(config.ImagesFolderPath);
            _pdfPagesDir = new DirectoryInfo(config.PdfPagesFolderPath);
            _recipesDir = new DirectoryInfo(config.RecipesFolderPath);
            _cachedRecipes = new List<Recipe>();
            _jsonOptions = JsonOptionsProvider.GetModelsJsonOptions();
        }


        protected List<string> ListAvailableRecipes()
        {
            if(_recipesDir.Exists == false)
            {
                return new List<string>();
            }

            var outList = new List<string>();
            foreach (var element in _recipesDir.GetFiles("*.json"))
            {
                outList.Add(element.FullName);
            }
            return outList;
        }

        protected async Task<List<Recipe>> ParseFromIndividualRecipe(FileInfo[] files)
        {
            var allRecipesList = new List<Recipe>();
            foreach (var recipeFile in files)
            {
                var file = recipeFile.OpenRead();
                var parsedRecipe = await JsonSerializer.DeserializeAsync<Recipe>(file, _jsonOptions);

                if (parsedRecipe != null)
                {
                    allRecipesList.Add(parsedRecipe);
                }
            }

            return allRecipesList;
        }

        public async Task<List<Recipe>> GetAllRecipesAsync(bool noCaching = false)
        {
            // Speeding up subsequent calls
            if(_cachedRecipes!= null && _cachedRecipes.Count != 0 && noCaching == false)
            {
                return _cachedRecipes;
            }
            
            var allRecipesList = new List<Recipe>();
            
            var availableRecipes = _recipesDir.GetFiles("*.json");
            var allRecipesMonoFile = availableRecipes.First<FileInfo>(f => f.Name == "all_recipes.json");
            if (allRecipesMonoFile != null)
            {
                var file = allRecipesMonoFile.OpenRead();
                var parsedAllRecipesList = await JsonSerializer.DeserializeAsync<AllRecipesCollection>(file, _jsonOptions);
                file.Close();

                if(parsedAllRecipesList != null)
                {
                    allRecipesList = parsedAllRecipesList.Recipes;
                }
                // If the monolithic file has parsing issues, 
                // Try with the individual ones if we happen to have them at hands as well
            }
            
            
            // If at this point the allRecipesList is still empty, try to read individual files
            // from disk
            if(allRecipesList.Count == 0)
            {
                allRecipesList = await ParseFromIndividualRecipe(availableRecipes);
            }

            // Don't use cache, this will force subsequent calls to perform Disk Access
            // And it won't fill the RAM
            // Note that this is not a "Stateless" server anymore if we do caching, 
            // But as this is general "public" data which will be available to anyone in read only mode, 
            // We can cache it and speed up response times.
            if(noCaching == false)
            {
                _cachedRecipes = allRecipesList;
            }
            return allRecipesList;
        }

        protected async Task<Recipe?> ReadSingleRecipeAsync(FileInfo fileSource)
        {
            var file = fileSource.OpenRead();
            var recipe = await JsonSerializer.DeserializeAsync<Recipe>(file, _jsonOptions);
            file.Close();
            return recipe;
        }

        public async Task<Recipe?> GetRecipeByNumberAsync(uint number, bool noCaching = false)
        {
            if(_cachedRecipes != null && _cachedRecipes.Count != 0 && noCaching == false)
            {
                var cachedRecipe = _cachedRecipes.First<Recipe>(r => r.Number == number);
                if(cachedRecipe != null)
                {
                    return cachedRecipe;
                }
            }

            var matchingFile = _recipesDir.GetFiles("*.json").First(f =>
            {
                var right = f.Name.Split('_')[1];
                var decodedNumber = uint.Parse(right.Replace(".json", ""));
                return decodedNumber == number;
            });
            
            Recipe? recipe = null;
            if(matchingFile != null) 
            {
                recipe = await ReadSingleRecipeAsync(matchingFile);
            }
            return recipe;
        }

        protected Recipe? FindByName(string name, List<Recipe> recipeList)
        {   
            return recipeList.First(r => r.Name == name);
        }

        public async Task<Recipe?> GetRecipeByNameAsync(string name, bool noCaching = false)
        {
            if (_cachedRecipes != null && _cachedRecipes.Count != 0)
            {
                var cachedRecipe = FindByName(name, _cachedRecipes);
                if (cachedRecipe != null)
                {
                    return cachedRecipe;
                }
            }

            var allRecipes = await GetAllRecipesAsync(noCaching);
            var matchingRecipe = FindByName(name, allRecipes);
            return matchingRecipe;
        }
    }
}
