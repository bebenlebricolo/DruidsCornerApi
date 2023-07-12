using Moq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using DruidsCornerAPI.Models.Search;
using DruidsCornerAPI.Services;
using NUnit.Framework.Internal;
using DruidsCornerAPI.DatabaseHandlers;
using DruidsCornerAPI.Models.Config;

namespace DruidsCornerUnitTests.Services
{

    public class SearchServiceTest
    {
        private DirectoryInfo? _localTestDbFolder;
        private IConfiguration _fakeConfig;
        private LocalDatabaseHandler _localDbHandler;

        [SetUp]
        public void Setup()
        {
            _localTestDbFolder = TestHelpers.TestDatabaseFinder.FindTestDatabase();
            _fakeConfig = TestHelpers.ConfiHelper.GenerateEmptyFakeConfig();

            // Provide a local database handler
            var mockDbLogger = new Mock<ILogger<LocalDatabaseHandler>>();
            var deployedConfig = new DeployedDatabaseConfig();
            deployedConfig.FromRootFolder(_localTestDbFolder!.FullName);
            _localDbHandler = new LocalDatabaseHandler(deployedConfig, mockDbLogger.Object);
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
        public async Task TestSearchGetSingleProp()
        {
            var mockLogger = new Mock<ILogger<SearchService>>();
            var searchService = new SearchService(_fakeConfig, mockLogger.Object);

            var hopsQueryList = new List<string>{
                "Ahtanum",
                "Amarillo"
            };

            var reversedHopMapping = await _localDbHandler.GetIndexedHopDbAsync();
            var candidates = await _localDbHandler.GetAllRecipesAsync();
            var recipes = searchService.GetMatchingRecipeByHops(candidates, reversedHopMapping, hopsQueryList);
            Assert.That(recipes.Count, Is.EqualTo(3));
        }
    }
}