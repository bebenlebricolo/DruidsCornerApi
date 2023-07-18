using DruidsCornerAPI.Tools;

namespace DruidsCornerAPI.Models.DiyDog.References
{
    /// <summary>
    /// Very simple Known Good Yeasts list
    /// </summary>
    public class ReferenceYeasts
    {
        /// <summary>
        /// List of yeast properties
        /// </summary>
        public List<YeastProperty> Yeasts {get; set; } = new List<YeastProperty>();  

        /// <summary>
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Equals(object? other)
        {
            if(other is not ReferenceYeasts)
            {
                return false;
            }
            return Equals(other as ReferenceYeasts);
        }
        
        /// <summary>
        /// Custom comparison operators
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(ReferenceYeasts? other)
        {
            bool identical = other is not null;
            if(!identical) return false;
            
            identical &= Language.CompareEquivalentLists(Yeasts, other!.Yeasts);
            return identical;
        }

        /// <summary>
        /// Custom equality operator
        /// </summary>
        public static bool operator == (ReferenceYeasts? left, ReferenceYeasts? right)
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
        public static bool operator != (ReferenceYeasts? left, ReferenceYeasts? right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Custom hasher
        /// </summary>
        public override int GetHashCode()
        {
            var hash = Yeasts.GetHashCode() * 3;
            return hash;
        }
    }
}