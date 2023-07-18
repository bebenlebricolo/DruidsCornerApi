using DruidsCornerAPI.Models.DiyDog.RecipeDb;
using DruidsCornerAPI.Tools;

namespace DruidsCornerAPI.Models.DiyDog.References
{
    /// <summary>
    /// Very simple Known Good Malts list
    /// </summary>
    public class ReferenceMalts
    {
        /// <summary>
        /// List of Malt properties
        /// </summary>
        public List<MaltProperty> Malts {get; set; } = new List<MaltProperty>();  

        /// <summary>
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Equals(object? other)
        {
            if(other is not ReferenceMalts)
            {
                return false;
            }
            return Equals(other as ReferenceMalts);
        }

        /// <summary>
        /// Custom comparison operators
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(ReferenceMalts? other)
        {
            bool identical = other is not null;
            if(!identical) return false;
            
            identical &= Language.CompareEquivalentLists(Malts, other!.Malts);
            return identical;
        }

        /// <summary>
        /// Custom equality operator
        /// </summary>
        public static bool operator == (ReferenceMalts? left, ReferenceMalts? right)
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
        public static bool operator != (ReferenceMalts? left, ReferenceMalts? right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Custom hasher
        /// </summary>
        public override int GetHashCode()
        {
            var hash = Malts.GetHashCode() * 3;
            return hash;
        }
        
    }
}