using DruidsCornerAPI.Models;
using DruidsCornerAPI.Models.DiyDog;
using DruidsCornerAPI.Tools;
using System.Text.Json;

namespace DruidsCornerUnitTests.Models.Google
{
    public class RecipeTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void CheckJsonSymetry()
        {
            var fakeBeer = new Recipe()
            {
                Basics = new Basics
                {
                  Abv = 5.0f,
                  AttenuationLevel = 75.0f,
                  Volume = new Volume { Litres = 20, Galons = 5},
                  BoilVolume = new Volume { Galons = 6.6f, Litres = 25},
                  Ebc = 60.0f,
                  Ibu = 50, 
                  Ph = 4.4f,
                  Srm = 40.0f,
                  TargetFg = 1010.0f,
                  TargetOg = 1055.0f
                },
                BrewersTip = "Leave it outside !",
                Description = "description",
                FirstBrewed = "01/01/1970",
                FoodPairing = new List<string>() { "Chocolate cake"},
                Image = new FileRecord()
                {
                    Path = "Somewhere"
                },
                PdfPage = new FileRecord()
                {
                    Path = "Somewhere else"
                },
                Ingredients = new Ingredients()
                {
                    AlternativeDescription = "alternative description",
                    Hops = new List<Hop>()
                    { 
                        new Hop() {Amount = 12, Attribute = "Bittering", Name = "Magnum", When = "Start"},
                        new Hop() {Amount = 23, Attribute = "Bittering", Name = "Nelson Sauvin", When = "Middle"},
                        new Hop() {Amount = 34, Attribute = "Aroma", Name = "Hallertau Perle", When = "End"}
                    },
                    Malts = new List<Malt>
                    { 
                        new Malt {Name = "Pilsner", Kgs = 5.0f },
                        new Malt {Name = "Caramalt", Kgs = 0.5f }, 
                        new Malt {Name = "Carafa Special II", Kgs = 0.1f } 
                    },
                    Yeasts = new List<Yeast>
                    { 
                        new Yeast {Name = "Wyeast American Ale 1056", ManufacturerLink = "https://wyeastlab.com/product/american-ale/"}
                    }
                },
                Name = "Fake name",
                Number = 123456,
                PackagingType = PackagingType.Squirrel,
                ParsingErrors = new List<string> { "One error !"},
                Subtitle = "Fake subtitle",
                MethodTimings = new MethodTimings() {
                    Fermentation = {Celsius = 20, Fahrenheit = 77, Time = 30}, 
                    MashTemps = new List<MashTemp>() { new MashTemp() { Celsius = 66, Fahrenheit = 150, Time = 60} },
                    MashTips = new List<string> { "Yay!"},
                    Twists = new List<Twist>() { new Twist{ Amount = 12, Name = "Mango chips", When = "Dry hop" } },
                },
                Tags = {"Weird experiment", "Unqualifiable beer"},
            };

            var jsonOptions = JsonOptionsProvider.GetModelsJsonOptions();
            var serialized = JsonSerializer.Serialize(fakeBeer, jsonOptions);
            var deserialized = JsonSerializer.Deserialize<Recipe>(serialized, jsonOptions);

            fakeBeer.Equals(deserialized);


            fakeBeer.Description = "";
        }
    }
}