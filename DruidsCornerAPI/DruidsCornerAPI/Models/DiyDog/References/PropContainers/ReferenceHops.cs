using System.Text.Json.Serialization;
using DruidsCornerAPI.Tools;
using Microsoft.AspNetCore.Mvc.Razor;

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
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Equals(object? other)
        {
            if(other is not ReferenceHops)
            {
                return false;
            }
            return Equals(other as ReferenceHops);
        }

        
        /// <summary>
        /// Custom comparison operators
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(ReferenceHops? other)
        {
            bool identical = other is not null;
            if(!identical) return false;
            
            // We don't care about the order here, we are looking for the same numbers and nothing more.
            identical &= Language.CompareEquivalentLists(Hops, other!.Hops);
            return identical;
        }

        /// <summary>
        /// Custom equality operator
        /// </summary>
        public static bool operator == (ReferenceHops? left, ReferenceHops? right)
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
        public static bool operator != (ReferenceHops? left, ReferenceHops? right)
        {
            return !(left == right);
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