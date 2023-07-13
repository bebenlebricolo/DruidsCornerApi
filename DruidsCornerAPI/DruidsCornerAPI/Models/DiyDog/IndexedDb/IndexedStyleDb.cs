using DruidsCornerAPI.Tools;

namespace DruidsCornerAPI.Models.DiyDog.IndexedDb
{
    /// <summary>
    /// Encodes basic information about BrewDog's beer recipe.
    /// </summary>
    public class IndexedStyleDb : IndexedDb
    {
        /// <summary>
        /// List of properties in a reversed DB construct
        /// </summary>
        public List<ReversedPropMapping> Styles {get; set;} = new List<ReversedPropMapping>();  

        /// <summary>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj)
        {
            if(obj is not IndexedStyleDb)
            {
                return false;
            }
            return Equals(obj as IndexedStyleDb);
        }

        /// <summary>
        /// Custom comparison operators
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(IndexedStyleDb? other)
        {
            bool identical = other  is not null;
            if(!identical) return false;

            identical &= Language.CompareEquivalentLists(Styles, other!.Styles);
            return identical;
        }

        /// <summary>
        /// Custom equality operator
        /// </summary>
        public static bool operator == (IndexedStyleDb? left, IndexedStyleDb? right)
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
        public static bool operator != (IndexedStyleDb? left, IndexedStyleDb? right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Custom hasher
        /// </summary>
        public override int GetHashCode()
        {
            return Styles.GetHashCode() * 3;
        }
    }
}
