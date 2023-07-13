using DruidsCornerAPI.Models;
using DruidsCornerAPI.Models.DiyDog.RecipeDb;
using DruidsCornerAPI.Models.DiyDog.References;
using DruidsCornerAPI.Tools;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace DruidsCornerUnitTests.Models.Databases
{
    public class ReferenceDbTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestReferenceHopsJsonSymmetry()
        {
            var refHops = new ReferenceHops() {
                Hops = new List<HopProperty>() {
                    new HopProperty()
                    {
                        Aliases = null,
                        Name = "Hop 1",
                        Url = "fake url 1"
                    },
                    new HopProperty()
                    {
                        Aliases = new List<string>{"Alias 1", "Alias2"},
                        Name = "Hop 2",
                        Url = null
                    },
                    new HopProperty()
                    {
                        Aliases = null,
                        Name = "Hop 3",
                        Url = null
                    },
                    new HopProperty()
                    {
                        Aliases = new List<string>{"some alias", "some other alias"},
                        Name = "Hop 4",
                        Url = "Some other url"
                    },
                }
            };

            var jsonOptions = JsonOptionsProvider.GetModelsJsonOptions();
            var serialized = JsonSerializer.Serialize(refHops, jsonOptions);
            var deserialized = JsonSerializer.Deserialize<ReferenceHops>(serialized, jsonOptions);

            Assert.That(refHops, Is.EqualTo(deserialized));
            
        }

        [Test]
        public void TestReferenceYeastsJsonSymmetry()
        {
            var refYeasts = new ReferenceYeasts() {
                Yeasts = new List<YeastProperty>() {
                    new YeastProperty()
                    {
                        Aliases = null,
                        Manufacturer = "Someone",
                        Name = "Yeast 1",
                        Url = "fake url 1"
                    },
                    new YeastProperty()
                    {
                        Aliases = new List<string>(){"1056", "Americal Ale"},
                        Manufacturer = "Someone else",
                        Name = "Yeast 2",
                        Url = "fake url 1"
                    }
                }
            };

            var jsonOptions = JsonOptionsProvider.GetModelsJsonOptions();
            var serialized = JsonSerializer.Serialize(refYeasts, jsonOptions);
            var deserialized = JsonSerializer.Deserialize<ReferenceYeasts>(serialized, jsonOptions);

            Assert.That(refYeasts, Is.EqualTo(deserialized));
        }


        [Test]
        public void TestReferenceMaltsJsonSymmetry()
        {
            var refMalts = new ReferenceMalts() {
                Malts = new List<MaltProperty>() {
                    new MaltProperty()
                    {
                        Aliases = null,
                        Manufacturer = "Someone",
                        Name = "Malt1 1",
                        Url = "fake url 1"
                    },
                    new MaltProperty()
                    {
                        Aliases = new List<string>(){"Dark", "Chocolate"},
                        Manufacturer = "Someone else",
                        Name = "Malt 2",
                        Url = "fake url 2"
                    }
                }
            };

            var jsonOptions = JsonOptionsProvider.GetModelsJsonOptions();
            var serialized = JsonSerializer.Serialize(refMalts, jsonOptions);
            var deserialized = JsonSerializer.Deserialize<ReferenceMalts>(serialized, jsonOptions);

            Assert.That(refMalts, Is.EqualTo(deserialized));
        }

         [Test]
        public void TestReferenceStylesJsonSymmetry()
        {
            var refStyles = new ReferenceStyles() {
                Styles = new List<StyleProperty>() {
                    new StyleProperty()
                    {
                        Name = "Style 1",
                        Category = "Category 1",
                        Url = "fake url 1"
                    },
                    new StyleProperty()
                    {
                        Name = "Style 2",
                        Category = "Category 2",
                        Url = "fake url 2"
                    },
                }
            };

            var jsonOptions = JsonOptionsProvider.GetModelsJsonOptions();
            var serialized = JsonSerializer.Serialize(refStyles, jsonOptions);
            var deserialized = JsonSerializer.Deserialize<ReferenceStyles>(serialized, jsonOptions);

            Assert.That(refStyles, Is.EqualTo(deserialized));
        }
    }
}