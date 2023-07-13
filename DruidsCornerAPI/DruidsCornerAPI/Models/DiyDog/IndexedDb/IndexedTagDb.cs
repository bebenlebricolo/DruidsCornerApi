using DruidsCornerAPI.Tools;

namespace DruidsCornerAPI.Models.DiyDog.IndexedDb
{
    /// <summary>
    /// Encodes basic information about BrewDog's beer recipe.
    /// </summary>
    public class IndexedTagDb : IndexedDb
    {
        /// <summary>
        /// List of properties in a reversed DB construct
        /// </summary>
        public List<ReversedPropMapping> Tags {get; set;} = new List<ReversedPropMapping>();  
        
        /// <summary>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj)
        {
            if(obj is not IndexedTagDb)
            {
                return false;
            }
            return Equals(obj as IndexedTagDb);
        }

        /// <summary>
        /// Custom comparison operators
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(IndexedTagDb? other)
        {
            bool identical = other  is not null;
            if(!identical) return false;

            identical &= Language.CompareEquivalentLists(Tags, other!.Tags);
            return identical;
        }

         /// <summary>
        /// Custom equality operator
        /// </summary>
        public static bool operator == (IndexedTagDb? left, IndexedTagDb? right)
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
        public static bool operator != (IndexedTagDb? left, IndexedTagDb? right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Custom hasher
        /// </summary>
        public override int GetHashCode()
        {
            return Tags.GetHashCode() * 3;
        }
    }
}
