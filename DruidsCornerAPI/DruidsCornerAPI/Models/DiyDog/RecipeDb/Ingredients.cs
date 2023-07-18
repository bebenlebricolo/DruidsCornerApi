using DruidsCornerAPI.Tools;

namespace DruidsCornerAPI.Models.DiyDog.RecipeDb
{
    /// <summary>
    /// Ingredients datastructure 
    /// </summary>
    public class Ingredients 
    {
        /// <summary>
        /// List of malts used for a single recipe
        /// </summary>
        public List<Malt> Malts { get; set; } = new List<Malt>();

        /// <summary>
        /// List of hops used for a single recipe
        /// </summary>
        public List<Hop> Hops { get; set; } = new List<Hop>();


        /// <summary>
        /// Optional list of extra boil / fermentation ingredient
        /// </summary>
        public List<ExtraBoil>? ExtraBoil{ get; set; } = null;
        
        /// <summary>
        /// Optional list of extra mash ingredients
        /// </summary>
        public List<ExtraMash>? ExtraMash{ get; set; } = null;


        /// <summary>
        /// List of yeast used for a single recipe
        /// </summary> 
        public List<Yeast> Yeasts { get; set; } = new List<Yeast>();

        /// <summary>
        /// Alternative optional description, sometimes on some recipes an additional description was given instead of ingredients (...)
        /// </summary>
        public string? AlternativeDescription { get; set; } = null;

        /// <summary>
        /// Custom comparison operators
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj)
        {
            if(obj is not Ingredients)  
            {
                return false;
            }
            return Equals(obj as Ingredients);
        }

        /// <summary>
        /// Custom comparison operators
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Ingredients? other)
        {  
            bool identical = other is not null;
            identical &= Language.CompareEquivalentLists(Malts, other!.Malts);
            identical &= Language.CompareEquivalentLists(Hops, other!.Hops);
            identical &= Language.CompareEquivalentLists(ExtraBoil, other!.ExtraBoil);
            identical &= Language.CompareEquivalentLists(ExtraMash, other!.ExtraMash);
            identical &= Language.CompareEquivalentLists(Yeasts, other!.Yeasts);
            identical &= AlternativeDescription == other!.AlternativeDescription;

            return identical;
        }

        /// <summary>
        /// Custom equality operator
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator==(Ingredients? left, Ingredients? right)
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
        /// Custom inequality operator
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator!=(Ingredients? left, Ingredients? right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Custom hasher
        /// </summary>
        public override int GetHashCode()
        {
            var hash = Malts.GetHashCode() * 3;
            hash += Hops.GetHashCode() * 3;
            if(ExtraBoil != null)
            {
                hash += ExtraBoil.GetHashCode() * 3;
            }
            if(ExtraMash != null)
            {
                hash += ExtraMash.GetHashCode() * 3;
            }
            hash += Yeasts.GetHashCode() * 3;
            if(AlternativeDescription != null)
            {
                hash += AlternativeDescription.GetHashCode() * 5;
            }
            return hash;
        }

    }
}
