using DruidsCornerAPI.Tools;

namespace DruidsCornerAPI.Models.DiyDog.IndexedDb
{

    /// <summary>
    /// Encodes a simple reversed property mapping as found in {prop}_rv_db.json provided by 
    /// DiyDogExtract databases
    /// </summary>
    public class ReversedPropMapping
    {
        /// <summary>
        /// Property name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// List of recipes ordered by their number / id in which this property was found.
        /// </summary>
        public List<uint> FoundInBeers { get; set; } = new List<uint>();

        /// <summary>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj)
        {
            if(obj is not ReversedPropMapping)
            {
                return false;
            }
            return Equals(obj as ReversedPropMapping);
        }

        /// <summary>
        /// Custom comparison operators
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(ReversedPropMapping? other)
        {
            bool identical = other != null;
                      
            identical &= Name == other!.Name;
            identical &= Language.CompareEquivalentLists(FoundInBeers, other!.FoundInBeers);
            return identical;
        }

        
        /// <summary>
        /// Custom equality operator
        /// </summary>
        public static bool operator == (ReversedPropMapping? left, ReversedPropMapping? right)
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
        public static bool operator != (ReversedPropMapping? left, ReversedPropMapping? right)
        {
            return !(left == right);
        }
        
        /// <summary>
        /// Custom hasher
        /// </summary>
        public override int GetHashCode()
        {
            return Name.GetHashCode() * 32 + FoundInBeers.GetHashCode() * 11;
        }
    }

}