using System.Text.Json;

using DruidsCornerAPI.Tools;
using DruidsCornerAPI.Models.Config;
using DruidsCornerAPI.Models.Search;
using DruidsCornerAPI.Models.DiyDog.RecipeDb;
using DruidsCornerAPI.Models.DiyDog.References;
using DruidsCornerAPI.Models.DiyDog.IndexedDb;


namespace DruidsCornerAPI.DatabaseHandlers
{
    /// <summary>
    /// Local database handler class handles File based local database (DiyDog database)
    /// </summary>
    public class LocalDatabaseHandler : IDatabaseHandler
    {
        private DeployedDatabaseConfig _dbConfig;
        private ILogger<LocalDatabaseHandler> _logger;

        private List<Recipe> _cachedRecipes;
        private JsonSerializerOptions _jsonOptions;

        /// <summary>
        /// Constructs a LocalDatabaseHandler using a given DeployedDatabaseConfig as an input (points to folders)
        /// </summary>
        /// <param name="config">Deployed database configuration (used to read from disk)</param>
        public LocalDatabaseHandler(DeployedDatabaseConfig config, ILogger<LocalDatabaseHandler> logger)
        {
            _dbConfig = config;
            _logger = logger;
            _cachedRecipes = new List<Recipe>();
            _jsonOptions = JsonOptionsProvider.GetModelsJsonOptions();
        }

        /// <summary>
        /// Lists all available recipes on disk using a wildcard pattern 
        /// </summary>
        /// <returns></returns>
        protected List<string> ListAvailableRecipes()
        {
            if(_dbConfig.GetRecipesFolder().Exists == false)
            {
                return new List<string>();
            }

            var outList = new List<string>();
            foreach (var element in _dbConfig.GetRecipesFolder().GetFiles("*.json"))
            {
                outList.Add(element.FullName);
            }
            return outList;
        }

        /// <summary>
        /// Uses a list of filesources as an input to extract all recipes data from permanent storage.
        /// </summary>
        /// <param name="files">List of filesources to be read</param>
        /// <returns></returns>
        protected async Task<List<Recipe>> ParseFromIndividualRecipesFilesAsync(FileInfo[] files)
        {
            var allRecipesList = new List<Recipe>();
            foreach (var recipeFile in files)
            {
                var file = recipeFile.OpenRead();
                var parsedRecipe = await JsonSerializer.DeserializeAsync<Recipe>(file, _jsonOptions);
                file.Close();

                if (parsedRecipe != null)
                {
                    allRecipesList.Add(parsedRecipe);
                }
            }

            return allRecipesList;
        }

        /// <summary>
        /// Asynchronously fetches all recipes from disk.
        /// </summary>
        /// <param name="noCaching">Disables automatic caching to save memory, but slows down data accesses</param>
        /// <returns></returns>
        public async Task<List<Recipe>> GetAllRecipesAsync(bool noCaching = false)
        {
            // Speeding up subsequent calls
            if(_cachedRecipes!= null && _cachedRecipes.Count != 0 && noCaching == false)
            {
                return _cachedRecipes;
            }

            var allRecipesList = new List<Recipe>();

            var availableRecipes = _dbConfig.GetRecipesFolder().GetFiles("*.json");
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
                allRecipesList = await ParseFromIndividualRecipesFilesAsync(availableRecipes);
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

        /// <summary>
        /// Asynchronously parses a single recipe from a file source.
        /// </summary>
        /// <param name="fileSource"></param>
        /// <returns></returns>
        protected async Task<Recipe?> ReadSingleRecipeAsync(FileInfo fileSource)
        {
            var file = fileSource.OpenRead();
            var recipe = await JsonSerializer.DeserializeAsync<Recipe>(file, _jsonOptions);
            file.Close();
            return recipe;
        }

        /// <summary>
        /// Asynchronously retrieves a recipe using its base number / id in the database.
        /// </summary>
        /// <param name="number"></param>
        /// <param name="noCaching"></param>
        /// <returns></returns>
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

            var matchingFile = _dbConfig.GetRecipesFolder().GetFiles("*.json").First(f =>
            {
                var right = f.Name.Split('_')[1];
                var numberString = right.Replace(".json", "");
                uint decoded = 0;
                if(uint.TryParse(numberString, out decoded))
                {
                    return decoded == number;
                }
                return false;
            });

            Recipe? recipe = null;
            if(matchingFile != null)
            {
                recipe = await ReadSingleRecipeAsync(matchingFile);
            }
            return recipe;
        }

        /// <summary>
        /// Finds a recipe using its name in a given collection (name is lowercased)
        /// </summary>
        /// <param name="name"></param>
        /// <param name="recipeList"></param>
        /// <returns></returns>
        protected RecipeResult FindByName(string name, List<Recipe> recipeList)
        {
            var fuzzyResult = FuzzySearch.SearchPartialRatio(name, recipeList, elem => elem.Name);
            return new RecipeResult(fuzzyResult.Item1, fuzzyResult.Item2);
        }

        /// <summary>
        /// Asynchronously retrieves a recipe using its name as a key (lowercased)
        /// </summary>
        /// <param name="name"></param>
        /// <param name="noCaching"></param>
        /// <returns></returns>
        public async Task<RecipeResult> GetRecipeByNameAsync(string name, bool noCaching = false)
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

        /// <summary>
        /// Asynchronously retrieves an Image for a single recipe
        /// </summary>
        /// <param name="number"></param>
        /// <param name="noCaching"></param>
        /// <returns></returns>
        public async Task<Stream?> GetRecipeImageAsync(uint number, bool noCaching = false)
        {
            var recipe = await GetRecipeByNumberAsync(number, noCaching);
            if (recipe != null)
            {
                // This path is a local path relative to Root Folder
                var filePath = new FileInfo((recipe!.Image as FileRecord)!.Path);
                var candidate = _dbConfig.GetImagesFolder().GetFiles(filePath.Name).First();
                if (candidate != null)
                {
                   return new BufferedStream(candidate.OpenRead());
                }
            }
            return null;
        }

        /// <summary>
        /// Asynchronously retrieves a PDF page for a single recipe
        /// </summary>
        /// <param name="number"></param>
        /// <param name="noCaching"></param>
        /// <returns></returns>
        public async Task<Stream?> GetRecipePdfPageAsync(uint number, bool noCaching = false)
        {
            var recipe = await GetRecipeByNumberAsync(number, noCaching);
            if (recipe != null)
            {
                // This path is a local path relative to Root Folder
                //var filePath = new FileInfo((recipe.PdfPage as FileRecord).Path);
                var baseName = $"page_{number}.pdf";
                var candidate = _dbConfig.GetPdfPagesFolder().GetFiles(baseName).First();
                if (candidate != null)
                {
                    return new BufferedStream(candidate.OpenRead());
                }
            }
            return null;
        }

        /// <summary>
        /// Read reference properties Json file from disk and return the deserialized objects
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filename"></param>
        /// <returns></returns>
        protected async Task<T?> ReadRefFromDiskAsync<T>(string filename) where T : class?
        {
            var filepath = _dbConfig.GetReferencesFolder().GetFiles(filename).First();
            if(filepath == null)
            {
                return null;
            }

            try 
            {
                var file = filepath.Open(FileMode.Open);
                var referenceHops = await JsonSerializer.DeserializeAsync<T>(file);
                file.Close();
                return referenceHops;
            }
            catch (Exception ex) 
            {
                _logger.LogError($"Could not read reference {filename} from disk : {ex.Message}");
            }
            return null;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ReferenceHops?> GetReferenceHopsAsync()
        {   
            return await ReadRefFromDiskAsync<ReferenceHops>("known_good_hops.json");
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ReferenceMalts?> GetReferenceMaltsAsync()
        {
            return await ReadRefFromDiskAsync<ReferenceMalts>("known_good_malts.json");
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ReferenceYeasts?> GetReferenceYeastsAsync()
        {
            return await ReadRefFromDiskAsync<ReferenceYeasts>("known_good_yeasts.json");
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ReferenceStyles?> GetReferenceStylesAsync()
        {
            return await ReadRefFromDiskAsync<ReferenceStyles>("known_good_styles.json");
        }


        /// <summary>
        /// Reads from local file
        /// </summary>
        /// <param name="kind"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        protected async Task<IndexedDb?> ReadFromFile(IndexedDbPropKind kind, FileInfo path)
        {
            var file = path.OpenRead();
            IndexedDb? indexedDb = null;
            try
            {
                switch(kind)
                {
                    case IndexedDbPropKind.Hops :
                        indexedDb = await JsonSerializer.DeserializeAsync<IndexedHopDb>(file, _jsonOptions);
                        break;

                    case IndexedDbPropKind.Malts :
                        indexedDb = await JsonSerializer.DeserializeAsync<IndexedMaltDb>(file, _jsonOptions);
                        break;

                    case IndexedDbPropKind.Styles :
                        indexedDb = await JsonSerializer.DeserializeAsync<IndexedStyleDb>(file, _jsonOptions);
                        break;

                    case IndexedDbPropKind.Tags :
                        indexedDb = await JsonSerializer.DeserializeAsync<IndexedTagDb>(file, _jsonOptions);
                        break;

                    case IndexedDbPropKind.FoodPairing :
                        indexedDb = await JsonSerializer.DeserializeAsync<IndexedFoodPairingDb>(file, _jsonOptions);
                        break;

                    default:
                        _logger.LogError($"Unsupported property name : {kind}");
                        return null;
                }
            }
            catch(Exception ex)
            {
                _logger.LogError($"Could not read IndexedDb from file : {ex.Message}");
            }
            file.Close();
            return indexedDb;
        }


        /// <summary>
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<IndexedDb?> GetIndexedDbAsync(IndexedDbPropKind kind)
        {
            string filename;
            switch(kind)
            {
                case IndexedDbPropKind.Hops :
                    filename = "hops_rv_db.json";
                    break;
                case IndexedDbPropKind.Malts :
                    filename = "malts_rv_db.json";
                    break;
                case IndexedDbPropKind.Styles :
                    filename = "styles_rv_db.json";
                    break;
                case IndexedDbPropKind.Tags :
                    filename = "tags_rv_db.json";
                    break;
                case IndexedDbPropKind.FoodPairing :
                    filename = "foodPairing_rv_db.json";
                    break;
                default:
                    _logger.LogError($"Unsupported property name : {kind}");
                    return null;
            }
        
            var filepath = _dbConfig.GetIndexedDbFolder().GetFiles(filename).First();
            if(filepath == null)
            {
                _logger.LogError($"Could not find Indexed DB file on disk");
                return null;
            }

            var indexedDb = await ReadFromFile(kind, filepath);
            return indexedDb;
        }
    }
}
