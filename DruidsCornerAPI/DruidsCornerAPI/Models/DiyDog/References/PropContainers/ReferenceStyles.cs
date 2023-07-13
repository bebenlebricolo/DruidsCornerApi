using DruidsCornerAPI.Tools;

namespace DruidsCornerAPI.Models.DiyDog.References
{
    /// <summary>
    /// Very simple Known Good Styles list
    /// </summary>
    public class ReferenceStyles
    {
        /// <summary>
        /// List of Style properties
        /// </summary>
        public List<StyleProperty> Styles {get; set; } = new List<StyleProperty>();  

        /// <summary>
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Equals(object? other)
        {
            if(other is not ReferenceStyles)
            {
                return false;
            }
            return Equals(other as ReferenceStyles);
        }

        /// <summary>
        /// Custom comparison operators
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(ReferenceStyles? other)
        {
            bool identical = other is not null;
            if(!identical) return false;
            
            identical &= Language.CompareEquivalentLists(Styles, other!.Styles);
            return identical;
        }

        /// <summary>
        /// Custom equality operator
        /// </summary>
        public static bool operator == (ReferenceStyles? left, ReferenceStyles? right)
        {
            if(Language.SameNullity(new [] {left, right}))
            {
                if(left is null)
                {
                    return true;
                }
                left!.Equals(right);
            }
            return false;
        }

        /// <summary>
        /// Custom inequality operator
        /// </summary>
        public static bool operator != (ReferenceStyles? left, ReferenceStyles? right)
        {
            return !(left == right);
        }


        /// <summary>
        /// Custom hasher
        /// </summary>
        public override int GetHashCode()
        {
            var hash = Styles.GetHashCode() * 3;
            return hash;
        }
        
    }
}