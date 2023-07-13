using DruidsCornerAPI.Models.DiyDog.RecipeDb;

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
        /// Custom comparison operators
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj)
        {
            var other = obj as ReferenceMalts;
            bool identical = other != null;
            identical &= Malts.Count == other!.Malts.Count;
            if(!identical) return false;
            
            // We don't care about the order here, we are looking for the same numbers and nothing more.
            int index = 0;
            while(index < Malts.Count && identical)
            {
                identical &= other.Malts!.Any(item => item == Malts[index]);
                index++;
            }
            return identical;
        }

        /// <summary>
        /// Custom equality operator
        /// </summary>
        public static bool operator == (ReferenceMalts left, ReferenceMalts right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Custom inequality operator
        /// </summary>
        public static bool operator != (ReferenceMalts left, ReferenceMalts right)
        {
            return !left.Equals(right);
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