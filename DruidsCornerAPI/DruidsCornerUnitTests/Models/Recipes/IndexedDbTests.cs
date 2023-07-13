using Castle.Components.DictionaryAdapter;
using DruidsCornerAPI.Models;
using DruidsCornerAPI.Models.DiyDog.IndexedDb;
using DruidsCornerAPI.Models.DiyDog.RecipeDb;
using DruidsCornerAPI.Models.DiyDog.References;
using DruidsCornerAPI.Tools;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace DruidsCornerUnitTests.Models.Databases
{
    public class IndexedDbTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestIndexedHopDbJsonSymmetry()
        {
            var indexedHopDb = new IndexedHopDb(){
                Hops = new List<ReversedPropMapping>(){
                    new ReversedPropMapping() {
                        FoundInBeers = new List<uint>(){1, 2, 3, 4, 5, 6, 7, 8, 9, 10},
                        Name = "Ahtanum"
                    },
                    new ReversedPropMapping() {
                        FoundInBeers = new List<uint>(){2, 4, 6, 8, 10},
                        Name = "Amarillo"
                    }
                }
            };

            var jsonOptions = JsonOptionsProvider.GetModelsJsonOptions();
            var serialized = JsonSerializer.Serialize(indexedHopDb, jsonOptions);
            var deserialized = JsonSerializer.Deserialize<IndexedHopDb>(serialized, jsonOptions);

            Assert.That(indexedHopDb, Is.EqualTo(deserialized));
        }

        [Test]
        public void TestIndexedYeastDbJsonSymmetry()
        {
            
            var indexedYeastDb = new IndexedYeastDb(){
                Yeasts = new List<ReversedPropMapping>()
                {
                    new ReversedPropMapping(){
                        FoundInBeers = new List<uint>(){1, 3, 5, 7, 9},
                        Name = "WYeast 1056"
                    },
                    new ReversedPropMapping(){
                        FoundInBeers = new List<uint>(){10, 11, 13, 15, 17},
                        Name = "WYeast 1272"
                    }
                }
            };

            var jsonOptions = JsonOptionsProvider.GetModelsJsonOptions();
            var serialized = JsonSerializer.Serialize(indexedYeastDb, jsonOptions);
            var deserialized = JsonSerializer.Deserialize<IndexedYeastDb>(serialized, jsonOptions);

            Assert.That(indexedYeastDb, Is.EqualTo(deserialized));
        }

        [Test]
        public void TestIndexedMaltDbJsonSymmetry()
        {
            
            var indexedMaltDb = new IndexedMaltDb(){
                Malts = new List<ReversedPropMapping>()
                {
                    new ReversedPropMapping(){
                        FoundInBeers = new List<uint>(){1, 3, 5, 7, 9},
                        Name = "Malt 1"
                    },
                    new ReversedPropMapping(){
                        FoundInBeers = new List<uint>(){10, 11, 13, 15, 17},
                        Name = "Malt 2"
                    }
                }
            };

            var jsonOptions = JsonOptionsProvider.GetModelsJsonOptions();
            var serialized = JsonSerializer.Serialize(indexedMaltDb, jsonOptions);
            var deserialized = JsonSerializer.Deserialize<IndexedMaltDb>(serialized, jsonOptions);

            Assert.That(indexedMaltDb, Is.EqualTo(deserialized));
        }

        [Test]
        public void TestIndexedStyleDbJsonSymmetry()
        {
            
            var indexedStyleDb = new IndexedStyleDb(){
                Styles = new List<ReversedPropMapping>()
                {
                    new ReversedPropMapping(){
                        FoundInBeers = new List<uint>(){1, 3, 5, 7, 9},
                        Name = "Style 1"
                    },
                    new ReversedPropMapping(){
                        FoundInBeers = new List<uint>(){10, 11, 13, 15, 17},
                        Name = "Style 2"
                    }
                }
            };

            var jsonOptions = JsonOptionsProvider.GetModelsJsonOptions();
            var serialized = JsonSerializer.Serialize(indexedStyleDb, jsonOptions);
            var deserialized = JsonSerializer.Deserialize<IndexedStyleDb>(serialized, jsonOptions);

            Assert.That(indexedStyleDb, Is.EqualTo(deserialized));
        }

        [Test]
        public void TestIndexedFoodPairingDbJsonSymmetry()
        {
            
            var indexedFoodPairingDb = new IndexedFoodPairingDb(){
                FoodPairing = new List<ReversedPropMapping>()
                {
                    new ReversedPropMapping(){
                        FoundInBeers = new List<uint>(){1, 3, 5, 7, 9},
                        Name = "FoodPairing 1"
                    },
                    new ReversedPropMapping(){
                        FoundInBeers = new List<uint>(){10, 11, 13, 15, 17},
                        Name = "FoodPairing 2"
                    }
                }
            };

            var jsonOptions = JsonOptionsProvider.GetModelsJsonOptions();
            var serialized = JsonSerializer.Serialize(indexedFoodPairingDb, jsonOptions);
            var deserialized = JsonSerializer.Deserialize<IndexedFoodPairingDb>(serialized, jsonOptions);

            Assert.That(indexedFoodPairingDb, Is.EqualTo(deserialized));
        }

    }
}