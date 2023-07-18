using System.Text.Json.Serialization;
using DruidsCornerAPI.Tools;
using Microsoft.AspNetCore.Mvc.Razor;

namespace DruidsCornerAPI.Models.DiyDog.References
{
    /// <summary>
    /// Very simple Known Good Hops list
    /// </summary>
    public class ReferenceExtras
    {
        /// <summary>
        /// List of Hop properties
        /// </summary>
        public List<ExtraProperty> Extras {get; set; }= new List<ExtraProperty>();  

        /// <summary>
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Equals(object? other)
        {
            if(other is not ReferenceExtras)
            {
                return false;
            }
            return Equals(other as ReferenceExtras);
        }

        
        /// <summary>
        /// Custom comparison operators
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(ReferenceExtras? other)
        {
            bool identical = other is not null;
            if(!identical) return false;
            
            // We don't care about the order here, we are looking for the same numbers and nothing more.
            identical &= Language.CompareEquivalentLists(Extras, other!.Extras);
            return identical;
        }

        /// <summary>
        /// Custom equality operator
        /// </summary>
        public static bool operator == (ReferenceExtras? left, ReferenceExtras? right)
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
        public static bool operator != (ReferenceExtras? left, ReferenceExtras? right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Custom hasher
        /// </summary>
        public override int GetHashCode()
        {
            var hash = Extras.GetHashCode() * 3;
            return hash;
        }
        
    }
}