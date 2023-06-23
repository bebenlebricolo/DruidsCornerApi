using Microsoft.EntityFrameworkCore.Design;
using System.Text.Json.Serialization;

namespace DruidsCornerAPI.Models.DiyDog
{
    public enum PackagingType
    {
        Bottle,
        BigBottle,
        Squirrel,
        Keg,
        Barrel,
        Can
    }

    public record Recipe
    {
        public string Name { get; set; } = "";

        public string Subtitle { get; set; } = "";

        public string Description { get; set; } = "";

        public uint Number { get; set; } = 0;

        public List<string> Tags { get; set; } = new List<string>();

        public string FirstBrewed { get; set; } = "";

        public string? BrewersTip { get; set; } = "";

        public Basics Basics { get; set; } = new Basics();

        public Ingredients Ingredients { get; set; } = new Ingredients();

        public MethodTimings MethodTimings { get; set; } = new MethodTimings();

        public PackagingType PackagingType { get; set; } = PackagingType.Bottle;

        public DataRecord Image { get; set; } = new FileRecord();

        public DataRecord PdfPage { get; set; } = new FileRecord();

        public List<string>? FoodPairing { get; set; } = new List<string>();

        public List<string>? ParsingErrors { get; set; } = null;
    }
}
