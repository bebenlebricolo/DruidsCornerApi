using DruidsCornerAPI.Tools;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration.EnvironmentVariables;

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
    public class Recipe
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

        /// <summary>
        /// Custom comparison operators
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj)
        {
            if(obj is not Recipe)  
            {
                return false;
            }
            return Equals(obj as Recipe);
        }

        /// <summary>
        /// Custom comparison operators
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Recipe? other)
        {  
            bool identical = other is not null;
            identical &= Name == other!.Name;
            identical &= Subtitle == other!.Subtitle;
            identical &= Style == other!.Style;
            identical &= Description == other!.Description;
            identical &= Number == other!.Number;
            identical &= FirstBrewed == other!.FirstBrewed;
            identical &= BrewersTip == other!.BrewersTip;
            if(!identical) return false;

            // We don't care about the ordering here
            identical &= Language.CompareEquivalentLists(Tags, other!.Tags);
            identical &= Language.CompareEquivalentLists(FoodPairing, other!.FoodPairing);
            identical &= Language.CompareEquivalentLists(ParsingErrors, other!.ParsingErrors);
            
            // Costlier comparisons can happen now
            identical &= Basics == other!.Basics;
            identical &= Ingredients == other!.Ingredients;
            identical &= MethodTimings == other!.MethodTimings;
            identical &= PackagingType == other!.PackagingType;
            identical &= Image == other!.Image;
            identical &= PdfPage == other!.PdfPage;
            return identical;
        }

        /// <summary>
        /// Customc equality operator
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator==(Recipe? left, Recipe? right)
        {
            if(Language.SameNullity(new [] {left, right}))
            {
                if(left is null)
                {
                    return true;
                }
                return left!.Equals(right);
            }
            return false;
        }
        
        /// <summary>
        /// Customc inequality operator
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator!=(Recipe? left, Recipe? right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Custom hasher
        /// </summary>
        public override int GetHashCode()
        {
            var hash = Name.GetHashCode() * 2;
            hash += Subtitle.GetHashCode() * 2;
            hash += Style.GetHashCode() * 2;
            hash += Description.GetHashCode() * 2;
            hash += Number.GetHashCode() * 2;
            hash += Tags.GetHashCode() * 2;
            hash += FirstBrewed.GetHashCode() * 2;
            
            if(BrewersTip is not null)
            {
                hash += BrewersTip.GetHashCode() * 2;
            }

            hash += Basics.GetHashCode() * 2;
            hash += Ingredients.GetHashCode() * 2;
            hash += MethodTimings.GetHashCode() * 2;
            hash += PackagingType.GetHashCode() * 2;
            hash += Image.GetHashCode() * 2;
            hash += PdfPage.GetHashCode() * 2;
            
            if(FoodPairing is not null)
            {
                hash += FoodPairing.GetHashCode() * 2;
            }
            
            if(ParsingErrors is not null)
            {
                hash += ParsingErrors.GetHashCode() * 2;
            }
            return hash;
        }
    }
}
