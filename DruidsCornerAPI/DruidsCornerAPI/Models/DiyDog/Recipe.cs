using Microsoft.EntityFrameworkCore.Design;
using System.Text.Json.Serialization;

namespace DruidsCornerAPI.Models.DiyDog
{
    public record Volume
    {
        public string Litres { get; set; } = "";
        public string Gallons { get; set; } = "";
    }

    public record Basics
    {
        public Volume Volume { get; set; } = new Volume();

        public Volume BoilVolume { get; set; } = new Volume();

        public float Abv { get; set; } = 0.0f;

        public float TargetOg { get; set; } = 0.0f;

        public float TargetfG { get; set; } = 0.0f;

        public float Ebc { get; set; } = 0.0f;

        public float Ibu { get; set; } = 0.0f;

        public float Srm { get; set; } = 0.0f;

        public float Ph { get; set; } = 0.0f;

        public float AttenuationLevel { get; set; } = 0.0f;

    }

    public record Malt
    {
        public string Name { get; set; } = "";

        public float Kgs { get; set; } = 0.0f;

        public float Lbs { get; set; } = 0.0f;
    }

    public record Hop
    {
        public string Name { get; set; } = "";

        public float Amount { get; set; } = 0.0f;

        public string When { get; set; } = "";

        public string Attribute { get; set; } = "";
    }

    public record Yeast
    {
        public string Name { get; set; } = "";

        public Uri? ManufacturerLink { get; set; } = null;
    }

    public record Ingredients
    {
        public List<Malt> Malts { get; set; } = new List<Malt>();

        public List<Hop> Hops { get; set; } = new List<Hop>();

        public List<Yeast> Yeasts { get; set; } = new List<Yeast>();

        public string? AlternativeDescription { get; set; } = null;
    }

    public record Temperature
    {
        public float Celsius { get; set; } = 0.0f;

        public float Farenheit { get; set; } = 0.0f;
    }

    public record MashTemp : Temperature
    {
        public float Time { get; set; } = 0.0f;
    }

    public record Fermentation : Temperature
    {
        public float? Time { get; set; } = null;

        public List<string> Tips { get; set; } = new List<string>();
    }

    public record Twist
    {
        public string Name { get; set; } = "";

        public float? Amount { get; set; } = null;

        public string? When { get; set; } = null;
    }

    public record MethodTimings
    {
        public List<MashTemp> MashTemps { get; set; } = new List<MashTemp>();

        public List<string> MashTips { get; set; } = new List<string>();

        public Fermentation Fermentation { get; set; } = new Fermentation();

        public List<Twist> Twists { get; set; } = new List<Twist>();
    }


    public enum PackagingType
    {
        Bottle,
        BigBottle,
        Squirrel,
        Keg,
        Barrel,
        Can
    }


    public class Recipe
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
