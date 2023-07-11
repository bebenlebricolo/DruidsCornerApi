using Moq;
using System.Collections.Generic;
using NUnit.Framework.Internal;

using DruidsCornerAPI.Models;
using DruidsCornerAPI.Tools;
using DruidsCornerAPI.DatabaseHandlers;
using DruidsCornerAPI.Models.Config;
using Microsoft.Extensions.Logging;
using DruidsCornerAPI.Models.DiyDog.IndexedDb;
using Microsoft.VisualBasic;
using DruidsCornerAPI.Tools.Logging;

namespace DruidsCornerUnitTests.DatabaseHandlers
{

    [TestFixture]
    public class LocalDatabaseHandlersTest
    {
        private DirectoryInfo _localTestDbFolder;


        [SetUp]
        public void Setup()
        {
            var testDir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            // Recurse until base project folder (...)
            while(testDir != null && testDir.Name != nameof(DruidsCornerAPI))
            {
                testDir = testDir.Parent;
            }

            var unitTestFolder = testDir!.GetDirectories("DruidsCornerUnitTests").First()!;
            _localTestDbFolder = unitTestFolder.GetDirectories("TestDatabase").First();
        }

        [Test]
        public async Task CanReadIndexedDbsFromDisk()
        {
            var mockLogger = new Mock<ILogger<LocalDatabaseHandler>>();

            // Lookup the local directory structure for this test database
            var config = new DeployedDatabaseConfig();
            Assert.That(config.FromRootFolder(_localTestDbFolder.FullName), Is.True);
            
            var handler = new LocalDatabaseHandler(config, mockLogger.Object);
            var indexedDb = await handler.GetIndexedDbAsync(IndexedDbPropKind.Malts);
            Assert.That(indexedDb, Is.Not.Null);
        }
    }
}