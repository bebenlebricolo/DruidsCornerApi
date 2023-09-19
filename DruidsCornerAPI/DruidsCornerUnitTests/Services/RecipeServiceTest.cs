using DruidsCornerAPI.Models;
using DruidsCornerAPI.DatabaseHandlers;
using DruidsCornerAPI.Models.Config;
using DruidsCornerAPI.Services;
using DruidsCornerAPI.Tools;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework.Internal;
using System.Collections.Generic;

namespace DruidsCornerUnitTests.Services
{

    public class RecipeServiceTest
    {
        private DirectoryInfo? _localTestDbFolder;
        private IConfiguration _fakeConfig;
        private LocalDatabaseHandler _localDbHandler;

        public RecipeServiceTest()
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
        public async Task TestCanGetRecipeByNumber()
        {
            var mockLogger = new Mock<ILogger<RecipeService>>();
            var service = new RecipeService(_fakeConfig, mockLogger.Object, _localDbHandler);

            var recipe = await service.GetRecipeByNumberAsync(0);
            Assert.That(recipe, Is.Null);

            for(uint i = 1 ; i < 6 ; i++)
            {
                recipe = await service.GetRecipeByNumberAsync(i);
                Assert.That(recipe, Is.Not.Null);
            }
            
            recipe = await service.GetRecipeByNumberAsync(8888888);
            Assert.That(recipe, Is.Null);
        }
    }
}