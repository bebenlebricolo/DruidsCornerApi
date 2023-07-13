using DruidsCornerAPI.Tools;

namespace DruidsCornerAPI.Models.DiyDog.RecipeDb
{
    /// <summary>
    /// Fermentation temperatures instructions
    /// </summary>
    public class Fermentation : Temperature
    {
        /// <summary>
        /// Optional time (used for extended fermentation steps, such as cask-aging)
        /// </summary>
        public float? Time { get; set; } = null;

        /// <summary>
        /// Optional additional tips indicated by the brewer's team
        /// </summary>
        public List<string> Tips { get; set; } = new List<string>();

        /// <summary>
        /// Custom comparison operators
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj)
        {
            if(obj is not Fermentation)  
            {
                return false;
            }
            return Equals(obj as Fermentation);
        }

        /// <summary>
        /// Custom comparison operators
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Fermentation? other)
        {  
            bool identical = other is not null;
            
            // We don't care about the ordering here
            identical &= Language.CompareEquivalentLists(Tips, other!.Tips);
            identical &= Time == other!.Time;
            return identical;
        }

        /// <summary>
        /// Customc equality operator
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator==(Fermentation? left, Fermentation? right)
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
        public static bool operator!=(Fermentation? left, Fermentation? right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Custom hasher
        /// </summary>
        public override int GetHashCode()
        {
            var hash = Tips.GetHashCode() * 2;
            if(Time != null)
            {
                hash += Time.GetHashCode() * 2;
            }
            return hash;
        }
    }
}
