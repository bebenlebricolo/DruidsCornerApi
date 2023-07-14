using Moq;
using NUnit.Framework.Internal;

using DruidsCornerAPI.DatabaseHandlers;
using DruidsCornerAPI.Models.Config;
using Microsoft.Extensions.Logging;
using DruidsCornerAPI.Models.DiyDog.IndexedDb;

namespace DruidsCornerUnitTests.DatabaseHandlers
{

    [TestFixture]
    public class LocalDatabaseHandlersTest
    {
        private DirectoryInfo? _localTestDbFolder;


        [SetUp]
        public void Setup()
        {
            _localTestDbFolder = TestHelpers.TestDatabaseFinder.FindTestDatabase();
        }

        [Test]
        public async Task TestCanReadIndexedDbsFromDisk()
        {
            var mockLogger = new Mock<ILogger<LocalDatabaseHandler>>();

            // Lookup the local directory structure for this test database
            var config = new DeployedDatabaseConfig();
            Assert.That(config.FromRootFolder(_localTestDbFolder!.FullName), Is.True);
            
            var handler = new LocalDatabaseHandler(config, mockLogger.Object);
            var indexedMaltDb = await handler.GetIndexedDbAsync(IndexedDbPropKind.Malts) as IndexedMaltDb;
            Assert.That(indexedMaltDb, Is.Not.Null);
            Assert.That(indexedMaltDb.Malts.Count, Is.EqualTo(18));
            Assert.That(indexedMaltDb.Malts[3].Name, Is.EqualTo("Black Malt"));
            Assert.That(indexedMaltDb.Malts[3].FoundInBeers.Contains(101), Is.True);
        
            var indexedHopDb = await handler.GetIndexedDbAsync(IndexedDbPropKind.Hops) as IndexedHopDb;
            Assert.That(indexedHopDb, Is.Not.Null);
            Assert.That(indexedHopDb.Hops.Count, Is.EqualTo(4));
            Assert.That(indexedHopDb.Hops[3].Name, Is.EqualTo("Ariana"));
            Assert.That(indexedHopDb.Hops[3].FoundInBeers.Contains(353), Is.True);

            var indexedStyleDb = await handler.GetIndexedDbAsync(IndexedDbPropKind.Styles) as IndexedStyleDb;
            Assert.That(indexedStyleDb, Is.Not.Null);
            Assert.That(indexedStyleDb.Styles.Count, Is.EqualTo(6));
            Assert.That(indexedStyleDb.Styles[5].Name, Is.EqualTo("A Blend Of Two Barrel-aged Imperial Saisons"));
            Assert.That(indexedStyleDb.Styles[5].FoundInBeers.Contains(89), Is.True);

            var indexedTagDb = await handler.GetIndexedDbAsync(IndexedDbPropKind.Tags) as IndexedTagDb;
            Assert.That(indexedTagDb, Is.Not.Null);
            Assert.That(indexedTagDb.Tags.Count, Is.EqualTo(6));
            Assert.That(indexedTagDb.Tags[5].Name, Is.EqualTo("5% Abv Raspberry Berlinerweisse"));
            Assert.That(indexedTagDb.Tags[5].FoundInBeers.Contains(299), Is.True);

            var indexedFoodPairingDb = await handler.GetIndexedDbAsync(IndexedDbPropKind.FoodPairing) as IndexedFoodPairingDb;
            Assert.That(indexedFoodPairingDb, Is.Not.Null);
            Assert.That(indexedFoodPairingDb.FoodPairing.Count, Is.EqualTo(18));
            Assert.That(indexedFoodPairingDb.FoodPairing[17].Name, Is.EqualTo("Almond and Chocolate Tart"));
            Assert.That(indexedFoodPairingDb.FoodPairing[17].FoundInBeers.Contains(414), Is.True);
        
            var wrongIndexedDbKey = await handler.GetIndexedDbAsync(IndexedDbPropKind.Unknown);
            Assert.That(wrongIndexedDbKey, Is.Null);

        }


        [Test]
        public async Task TestCanReadReferenceDbsFromDisk()
        {
            var mockLogger = new Mock<ILogger<LocalDatabaseHandler>>();
            // Lookup the local directory structure for this test database
            var config = new DeployedDatabaseConfig();
            Assert.That(config.FromRootFolder(_localTestDbFolder!.FullName), Is.True);
            
            var handler = new LocalDatabaseHandler(config, mockLogger.Object);
            var refHops = await handler.GetReferenceHopsAsync();
            Assert.That(refHops, Is.Not.Null);
            Assert.That(refHops.Hops.Count, Is.EqualTo(3));
            Assert.That(refHops.Hops[2].Name, Is.EqualTo("Apollo"));
            Assert.That(refHops.Hops[2].Url, Is.EqualTo("https://beermaverick.com/hop/apollo/"));
            Assert.That(refHops.Hops[2].Aliases, Is.Null);

            var refMalts = await handler.GetReferenceMaltsAsync();
            Assert.That(refMalts, Is.Not.Null);
            Assert.That(refMalts.Malts.Count, Is.EqualTo(3));
            Assert.That(refMalts.Malts[2].Name, Is.EqualTo("Black Malt"));
            Assert.That(refMalts.Malts[2].Manufacturer, Is.EqualTo("Simpsons"));
            Assert.That(refMalts.Malts[2].Url, Is.EqualTo("https://www.simpsonsmalt.co.uk/our-malts/black-malt/"));
         
            var refStyles = await handler.GetReferenceStylesAsync();
            Assert.That(refStyles, Is.Not.Null);
            Assert.That(refStyles.Styles.Count, Is.EqualTo(3));
            Assert.That(refStyles.Styles[2].Name, Is.EqualTo("Eisbock"));
            Assert.That(refStyles.Styles[2].Category, Is.EqualTo("Bocks"));
            Assert.That(refStyles.Styles[2].Url, Is.EqualTo("https://www.beeradvocate.com/beer/styles/36/"));
         
            var refYeasts = await handler.GetReferenceYeastsAsync();
            Assert.That(refYeasts, Is.Not.Null);
            Assert.That(refYeasts.Yeasts.Count, Is.EqualTo(3));
            Assert.That(refYeasts.Yeasts[2].Name, Is.EqualTo("Saflager - S-189"));
            Assert.That(refYeasts.Yeasts[2].Manufacturer, Is.EqualTo("Fermentis"));
            Assert.That(refYeasts.Yeasts[2].Aliases!.Contains("S189"), Is.True);
        }
    }
}