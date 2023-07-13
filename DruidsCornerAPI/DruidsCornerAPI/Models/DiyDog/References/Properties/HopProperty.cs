using DruidsCornerAPI.Tools;

namespace DruidsCornerAPI.Models.DiyDog.References
{
    /// <summary>
    /// Encodes a known good Hop property
    /// This is the base for all ReferenceProperties (aka "known good" properties in the DiyDogExtractor databases)
    /// </summary>
    public class HopProperty : BaseProperty
    {
        /// <summary>
        /// List of potential aliases for this property
        /// </summary>
        public List<string>? Aliases { get; set; } = null;

        /// <summary>
        /// Custom comparison operators
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj)
        {
            if(obj is not HopProperty || obj is null)
            {
                return false;
            }

            var other = obj as HopProperty;
            if(other == null) 
            {
                return false;
            }
            bool identical = true;
            
            identical &= base.Equals(other as BaseProperty);
            if(!identical) return false;

            // Reject non-matching nullity
            identical &= Language.SameNullity(new[] { Aliases, other.Aliases });
            if(!identical) return false;

            // We don't care about the order here, we are looking for the same numbers and nothing more.
            if(Aliases != null)
            {
                int index = 0;
                while(index < Aliases.Count && identical)
                {
                    identical &= other.Aliases!.Contains(Aliases[index]);
                    index++;
                }
            }

            return identical;
        }

        /// <summary>
        /// Custom equality operator
        /// </summary>
        public static bool operator == (HopProperty left, HopProperty right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Custom inequality operator
        /// </summary>
        public static bool operator != (HopProperty left, HopProperty right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Custom hasher
        /// </summary>
        public override int GetHashCode()
        {
            var hash = base.GetHashCode() * 3;
            if(Aliases != null)
            {
                hash *= Aliases.GetHashCode() * 4;
            }
            return hash;
        }
    }
}