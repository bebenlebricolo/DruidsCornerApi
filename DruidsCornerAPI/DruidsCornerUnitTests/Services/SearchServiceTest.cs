using Moq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using DruidsCornerAPI.Models.Search;
using DruidsCornerAPI.Services;
using NUnit.Framework.Internal;
using DruidsCornerAPI.DatabaseHandlers;
using DruidsCornerAPI.Models.Config;
using DruidsCornerAPI.Models.DiyDog.RecipeDb;

namespace DruidsCornerUnitTests.Services
{

    public class SearchServiceTest
    {
        private DirectoryInfo? _localTestDbFolder;
        private IConfiguration _fakeConfig;
        private LocalDatabaseHandler _localDbHandler;

        public SearchServiceTest()
        {
            _localTestDbFolder = TestHelpers.TestDatabaseFinder.FindTestDatabase();
            _fakeConfig = TestHelpers.ConfiHelper.GenerateEmptyFakeConfig();

            // Provide a local database handler
            var mockDbLogger = new Mock<ILogger<LocalDatabaseHandler>>();
            var deployedConfig = new DeployedDatabaseConfig();
            deployedConfig.FromRootFolder(_localTestDbFolder!.FullName);
            _localDbHandler = new LocalDatabaseHandler(deployedConfig, mockDbLogger.Object);
        }

        [SetUp]
        public void Setup()
        {
        }


        [Test]
        public async Task TestSearchSimpleQuery()
        {
            Queries query = new Queries()
            {
                HopList = new List<string> {
                    "Ahtanum",
                    "Amarillo"
                }
            };

            var mockLogger = new Mock<ILogger<SearchService>>();
            var searchService = new SearchService(_fakeConfig, mockLogger.Object);
            
            var recipes = await searchService.SearchRecipeAsync(query, _localDbHandler);
            Assert.That(recipes.Count, Is.EqualTo(3));
            Assert.That(recipes[0].Name, Is.EqualTo("Punk Ipa 2007 - 2010"));
        }

        [Test]
        public async Task TestSearchEmptyQuery()
        {
            Queries query = new Queries(){};

            var mockLogger = new Mock<ILogger<SearchService>>();
            var searchService = new SearchService(_fakeConfig, mockLogger.Object);
            
            var recipes = await searchService.SearchRecipeAsync(query, _localDbHandler);
            Assert.That(recipes.Count, Is.EqualTo(5));
        }

        [Test]
        public async Task TestSearchGetSingleProp_Hops()
        {
            var mockLogger = new Mock<ILogger<SearchService>>();
            var searchService = new SearchService(_fakeConfig, mockLogger.Object);

            var queryList = new List<string>{
                "Ahtanum",
                "Amarillo"
            };

            var reversedMappingDb = await _localDbHandler.GetIndexedHopDbAsync();
            var candidates = await _localDbHandler.GetAllRecipesAsync();
            Assert.That(candidates, Is.Not.Null);
            var recipes = searchService.GetMatchingRecipeByHops(candidates!, reversedMappingDb, queryList);
            Assert.That(recipes.Count, Is.EqualTo(3));
        }

        [Test]
        public async Task TestSearchGetSingleProp_Malts()
        {
            var mockLogger = new Mock<ILogger<SearchService>>();
            var searchService = new SearchService(_fakeConfig, mockLogger.Object);

            var queryList = new List<string>{
                "extra pale maris",
                "cara"
            };

            var reversedMappingDb = await _localDbHandler.GetIndexedMaltDbAsync();
            var candidates = await _localDbHandler.GetAllRecipesAsync();
            Assert.That(candidates, Is.Not.Null);

            var recipes = searchService.GetMatchingRecipeByMalts(candidates!, reversedMappingDb, queryList);
            Assert.That(recipes.Count, Is.EqualTo(5));
        }


        [Test]
        public async Task TestSearchGetSingleProp_Yeasts()
        {
            var mockLogger = new Mock<ILogger<SearchService>>();
            var searchService = new SearchService(_fakeConfig, mockLogger.Object);

            var queryList = new List<string>{
                "S 189"
            };

            var reversedMappingDb = await _localDbHandler.GetIndexedYeastDbAsync();
            var candidates = await _localDbHandler.GetAllRecipesAsync();
            Assert.That(candidates, Is.Not.Null);

            var recipes = searchService.GetMatchingRecipeByYeasts(candidates!, reversedMappingDb, queryList);
            Assert.That(recipes.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task TestSearchGetSingleProp_Styles()
        {
            var mockLogger = new Mock<ILogger<SearchService>>();
            var searchService = new SearchService(_fakeConfig, mockLogger.Object);

            var queryList = new List<string>{
                "Amber",
                "Twisted Stout"
            };

            var reversedMappingDb = await _localDbHandler.GetIndexedStyleDbAsync();
            var candidates = await _localDbHandler.GetAllRecipesAsync();
            Assert.That(candidates, Is.Not.Null);

            var recipes = searchService.GetMatchingRecipeByStyles(candidates!, reversedMappingDb, queryList);
            Assert.That(recipes.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task TestSearchGetSingleProp_ExtraBoil()
        {
            var mockLogger = new Mock<ILogger<SearchService>>();
            var searchService = new SearchService(_fakeConfig, mockLogger.Object);

            var queries = new Queries(){
                ExtraBoilList = new List<string>(){
                    "coffee"
                }
            };

            var allRecipes = await _localDbHandler.GetAllRecipesAsync();
            Assert.That(allRecipes, Is.Not.Null);

            var candidates = new List<Recipe>();
            foreach(var recipe in allRecipes!)
            {
                var candidate = searchService.FilterOutExtraBoilDiscrete(recipe, queries);
                if(candidate != null)
                {
                    candidates.Add(candidate);
                }
            }
            Assert.That(candidates.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task TestSearchGetSingleProp_ExtraMash()
        {
            var mockLogger = new Mock<ILogger<SearchService>>();
            var searchService = new SearchService(_fakeConfig, mockLogger.Object);

            var queries = new Queries(){
                ExtraMashList = new List<string>(){
                    "Honey"
                }
            };

            var allRecipes = await _localDbHandler.GetAllRecipesAsync();
            Assert.That(allRecipes, Is.Not.Null);

            var candidates = new List<Recipe>();
            foreach(var recipe in allRecipes!)
            {
                var candidate = searchService.FilterOutExtraMashDiscrete(recipe, queries);
                if(candidate != null)
                {
                    candidates.Add(candidate);
                }
            }
            Assert.That(candidates.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task TestSearchGetSingleProp_TwistList()
        {
            var mockLogger = new Mock<ILogger<SearchService>>();
            var searchService = new SearchService(_fakeConfig, mockLogger.Object);

            var queries = new Queries(){
                TwistList = new List<string>(){
                    "lactose",
                    "coffee"
                }
            };

            var allRecipes = await _localDbHandler.GetAllRecipesAsync();
            Assert.That(allRecipes, Is.Not.Null);

            var candidates = new List<Recipe>();
            foreach(var recipe in allRecipes!)
            {
                var candidate = searchService.FilterOutTwistsDiscrete(recipe, queries);
                if(candidate != null)
                {
                    candidates.Add(candidate);
                }
            }
            Assert.That(candidates.Count, Is.EqualTo(1));
        }
        

        [Test]
        public async Task TestSearchGetSingleProp_Abv()
        {
            var mockLogger = new Mock<ILogger<SearchService>>();
            var searchService = new SearchService(_fakeConfig, mockLogger.Object);

            var queries = new Queries(){
                Abv = new Range<float>(7.0f, 8.0f)
            };

            var allRecipes = await _localDbHandler.GetAllRecipesAsync();
            Assert.That(allRecipes, Is.Not.Null);

            var candidates = new List<Recipe>();
            foreach(var recipe in allRecipes!)
            {
                var candidate = searchService.FilterOutAbvDiscrete(recipe, queries);
                if(candidate != null)
                {
                    candidates.Add(candidate);
                }
            }
            Assert.That(candidates.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task TestSearchGetSingleProp_Ibu()
        {
            var mockLogger = new Mock<ILogger<SearchService>>();
            var searchService = new SearchService(_fakeConfig, mockLogger.Object);

            var queries = new Queries(){
                Ibu = new Range<float>(10.0f, 50.0f)
            };

            var allRecipes = await _localDbHandler.GetAllRecipesAsync();
            Assert.That(allRecipes, Is.Not.Null);

            var candidates = new List<Recipe>();
            foreach(var recipe in allRecipes!)
            {
                var candidate = searchService.FilterOutIbuDiscrete(recipe, queries);
                if(candidate != null)
                {
                    candidates.Add(candidate);
                }
            }
            Assert.That(candidates.Count, Is.EqualTo(4));
        }

        [Test]
        public async Task TestSearchGetSingleProp_Ebc()
        {
            var mockLogger = new Mock<ILogger<SearchService>>();
            var searchService = new SearchService(_fakeConfig, mockLogger.Object);

            var queries = new Queries(){
                Ebc = new Range<float>(10.0f, 50.0f)
            };

            var allRecipes = await _localDbHandler.GetAllRecipesAsync();
            Assert.That(allRecipes, Is.Not.Null);

            var candidates = new List<Recipe>();
            foreach(var recipe in allRecipes!)
            {
                var candidate = searchService.FilterOutEbcDiscrete(recipe, queries);
                if(candidate != null)
                {
                    candidates.Add(candidate);
                }
            }
            Assert.That(candidates.Count, Is.EqualTo(3));
        }

        [Test]
        public async Task TestSearchGetSingleProp_TargetOg()
        {
            var mockLogger = new Mock<ILogger<SearchService>>();
            var searchService = new SearchService(_fakeConfig, mockLogger.Object);

            var queries = new Queries(){
                TargetOg = new Range<float>(1030.0f, 1056.0f)
            };

            var allRecipes = await _localDbHandler.GetAllRecipesAsync();
            Assert.That(allRecipes, Is.Not.Null);

            var candidates = new List<Recipe>();
            foreach(var recipe in allRecipes!)
            {
                var candidate = searchService.FilterOutTargetOgDiscrete(recipe, queries);
                if(candidate != null)
                {
                    candidates.Add(candidate);
                }
            }
            Assert.That(candidates.Count, Is.EqualTo(4));
        }

        [Test]
        public async Task TestSearchGetSingleProp_TargetFg()
        {
            var mockLogger = new Mock<ILogger<SearchService>>();
            var searchService = new SearchService(_fakeConfig, mockLogger.Object);

            var queries = new Queries(){
                TargetFg = new Range<float>(1000.0f, 1015.0f)
            };

            var allRecipes = await _localDbHandler.GetAllRecipesAsync();
            Assert.That(allRecipes, Is.Not.Null);

            var candidates = new List<Recipe>();
            foreach(var recipe in allRecipes!)
            {
                var candidate = searchService.FilterOutTargetFgDiscrete(recipe, queries);
                if(candidate != null)
                {
                    candidates.Add(candidate);
                }
            }
            Assert.That(candidates.Count, Is.EqualTo(5));
        }

        [Test]
        public async Task TestSearchGetSingleProp_Ph()
        {
            var mockLogger = new Mock<ILogger<SearchService>>();
            var searchService = new SearchService(_fakeConfig, mockLogger.Object);

            var queries = new Queries(){
                Ph = new Range<float>(4.4f, 5.5f)
            };

            var allRecipes = await _localDbHandler.GetAllRecipesAsync();
            Assert.That(allRecipes, Is.Not.Null);

            var candidates = new List<Recipe>();
            foreach(var recipe in allRecipes!)
            {
                var candidate = searchService.FilterOutPhDiscrete(recipe, queries);
                if(candidate != null)
                {
                    candidates.Add(candidate);
                }
            }
            Assert.That(candidates.Count, Is.EqualTo(4));
        }

        [Test]
        public async Task TestSearchGetSingleProp_AttenuationLevel()
        {
            var mockLogger = new Mock<ILogger<SearchService>>();
            var searchService = new SearchService(_fakeConfig, mockLogger.Object);

            var queries = new Queries(){
                AttenuationLevel = new Range<float>(75.5f, 80.0f)
            };

            var allRecipes = await _localDbHandler.GetAllRecipesAsync();
            Assert.That(allRecipes, Is.Not.Null);

            var candidates = new List<Recipe>();
            foreach(var recipe in allRecipes!)
            {
                var candidate = searchService.FilterOutAttenuationLevelDiscrete(recipe, queries);
                if(candidate != null)
                {
                    candidates.Add(candidate);
                }
            }
            Assert.That(candidates.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task TestSearchGetSingleProp_MashTemps()
        {
            var mockLogger = new Mock<ILogger<SearchService>>();
            var searchService = new SearchService(_fakeConfig, mockLogger.Object);

            var queries = new Queries(){
                MashTemps = new Range<float>(65.0f, 65.0f)
            };

            var allRecipes = await _localDbHandler.GetAllRecipesAsync();
            Assert.That(allRecipes, Is.Not.Null);

            var candidates = new List<Recipe>();
            foreach(var recipe in allRecipes!)
            {
                var candidate = searchService.FilterOutMashTempsDiscrete(recipe, queries);
                if(candidate != null)
                {
                    candidates.Add(candidate);
                }
            }
            Assert.That(candidates.Count, Is.EqualTo(4));
        }

        [Test]
        public async Task TestSearchGetSingleProp_FermentationTemps()
        {
            var mockLogger = new Mock<ILogger<SearchService>>();
            var searchService = new SearchService(_fakeConfig, mockLogger.Object);

            var queries = new Queries(){
                FermentationTemps = new Range<float>(10.0f, 19.0f)
            };

            var allRecipes = await _localDbHandler.GetAllRecipesAsync();
            Assert.That(allRecipes, Is.Not.Null);

            var candidates = new List<Recipe>();
            foreach(var recipe in allRecipes!)
            {
                var candidate = searchService.FilterOutFermentationTempsDiscrete(recipe, queries);
                if(candidate != null)
                {
                    candidates.Add(candidate);
                }
            }
            Assert.That(candidates.Count, Is.EqualTo(4));
        }
    }
}