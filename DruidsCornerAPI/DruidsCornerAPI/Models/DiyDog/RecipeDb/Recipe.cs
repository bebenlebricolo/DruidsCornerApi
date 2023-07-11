using Microsoft.EntityFrameworkCore.Design;
using System.Text.Json.Serialization;

namespace DruidsCornerAPI.Models.DiyDog.RecipeDb
{
    /// <summary>
    /// Available packaging types for the beers
    /// </summary>
    public enum PackagingType
    {
        /// <summary> Beer packaged in a Normal Bottle (0.33 L bottles) </summary>
        Bottle,
        /// <summary> Beer packaged in a Big bottle (0.75 L bottles) </summary>
        BigBottle,
        /// <summary> Beer packaged in a Bottle ... in a Squirrel ... (0.33 L) </summary>
        Squirrel,
        /// <summary> Beer packaged in a Keg </summary>
        Keg,
        /// <summary> Beer packaged in a Barrel / Cask </summary>
        Barrel,
        /// <summary> Beer packaged in a Can </summary>
        Can
    }

    /// <summary>
    /// Depicts a Beer Recipe
    /// </summary> 
    public record Recipe
    {
        /// <summary>
        /// Advertized beer name
        /// </summary>
        /// <value></value>
        public string Name { get; set; } = "";

        /// <summary>
        /// Beer subtitle. May contain various information such as brewing partnership, adjectives, etc..
        /// </summary>
        /// <value></value>
        public string Subtitle { get; set; } = "";
        
        /// <summary>
        /// Beer's most probable style (might be inaccurate, coming from DiyDog book.)
        /// </summary>
        /// <value></value>
        public string Style { get; set; } = "";

        /// <summary>
        /// Extended description as per read from the book.
        /// </summary>
        /// <value></value>
        public string Description { get; set; } = "";

        /// <summary>
        /// Beer number (depicted as the #xxx tag on each page)
        /// </summary>
        public uint Number { get; set; } = 0;

        /// <summary>
        /// Beer tags, might contain lots of various data such as a potential style, annotations, adjectives, etc.
        /// </summary>
        public List<string> Tags { get; set; } = new List<string>();

        /// <summary>
        /// Date of first brew
        /// </summary>
        public string FirstBrewed { get; set; } = "";

        /// <summary>
        /// Optional brewer's tip annotation. Sometimes we can find useful information in there.
        /// </summary>
        public string? BrewersTip { get; set; } = "";


        /// <summary>
        /// Basic section of the beer (base characteristics, abv, ibu, ebc, etc.)
        /// </summary>
        public Basics Basics { get; set; } = new Basics();

        /// <summary>
        /// Beer's ingredients (everything, liquid or solid, that goes into the wort at some point of time !)
        /// </summary>
        public Ingredients Ingredients { get; set; } = new Ingredients();

        /// <summary>
        /// Mash, Boil and Fermentation indications about the "How to make this beer".
        /// </summary>
        public MethodTimings MethodTimings { get; set; } = new MethodTimings();

        /// <summary>
        /// Most probable packaging type of this beer. Guessed from the overall shape of the beer's picture.
        /// </summary>
        public PackagingType PackagingType { get; set; } = PackagingType.Bottle;

        /// <summary>
        /// Beer's image (can either point to a local file source or a cloud record.)
        /// </summary>
        public DataRecord Image { get; set; } = new FileRecord();

        /// <summary>
        /// Beer's original PDF page. Can either point to a local file source or a cloud record.
        /// This PDF page might be requested for the end user to countercheck some values, parsing is not 100% reliable! 
        /// </summary>
        public DataRecord PdfPage { get; set; } = new FileRecord();

        /// <summary>
        /// Optional list of food pairings
        /// </summary>
        public List<string>? FoodPairing { get; set; } = new List<string>();

        /// <summary>
        /// Optional list of parsing errors. Gives a hint about what went wrong in the beer and advertized that the 
        /// parsing quality / recipe depiction is not as good as it should be. 
        /// If any error is listed here, have a look to the original PDF page and double check everything.
        /// </summary>
        /// <value></value>
        public List<string>? ParsingErrors { get; set; } = null;
    }
}
