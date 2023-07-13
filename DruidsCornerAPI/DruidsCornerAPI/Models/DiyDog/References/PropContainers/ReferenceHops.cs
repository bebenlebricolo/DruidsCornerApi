using System.Text.Json.Serialization;

namespace DruidsCornerAPI.Models.DiyDog.References
{
    /// <summary>
    /// Very simple Known Good Hops list
    /// </summary>
    public class ReferenceHops
    {
        /// <summary>
        /// List of Hop properties
        /// </summary>
        [JsonPropertyName("hops")]
        public List<HopProperty> Hops {get; set; }= new List<HopProperty>();  

        /// <summary>
        /// Custom comparison operators
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj)
        {
            var other = obj as ReferenceHops;
            bool identical = other != null;
            identical &= Hops.Count == other!.Hops.Count;
            if(!identical) return false;
            
            // We don't care about the order here, we are looking for the same numbers and nothing more.
            int index = 0;
            while(index < Hops.Count && identical)
            {
                identical &= other.Hops!.Any(item => item == Hops[index]);
                index++;
            }
            return identical;
        }

        /// <summary>
        /// Custom equality operator
        /// </summary>
        public static bool operator == (ReferenceHops left, ReferenceHops right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Custom inequality operator
        /// </summary>
        public static bool operator != (ReferenceHops left, ReferenceHops right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Custom hasher
        /// </summary>
        public override int GetHashCode()
        {
            var hash = Hops.GetHashCode() * 3;
            return hash;
        }
        
    }
}