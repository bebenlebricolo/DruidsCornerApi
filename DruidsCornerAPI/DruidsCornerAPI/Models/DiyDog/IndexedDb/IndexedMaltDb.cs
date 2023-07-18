using DruidsCornerAPI.Tools;

namespace DruidsCornerAPI.Models.DiyDog.IndexedDb
{
    /// <summary>
    /// Encodes basic information about BrewDog's beer recipe.
    /// </summary>
    public class IndexedMaltDb : IndexedDb
    {
        /// <summary>
        /// List of properties in a reversed DB construct
        /// </summary>
        public List<ReversedPropMapping> Malts {get; set;} = new List<ReversedPropMapping>();  

        /// <summary>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj)
        {
            if(obj is not IndexedMaltDb)
            {
                return false;
            }
            return Equals(obj as IndexedMaltDb);
        }

        /// <summary>
        /// Custom comparison operators
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(IndexedMaltDb? other)
        {
            bool identical = other is not null;
            if(!identical) return false;

            identical &= Language.CompareEquivalentLists(Malts, other!.Malts);
            return identical;
        }

        /// <summary>
        /// Custom equality operator
        /// </summary>
        public static bool operator == (IndexedMaltDb? left, IndexedMaltDb? right)
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
        public static bool operator != (IndexedMaltDb? left, IndexedMaltDb? right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Custom hasher
        /// </summary>
        public override int GetHashCode()
        {
            return Malts.GetHashCode() * 3;
        }
    }
}
