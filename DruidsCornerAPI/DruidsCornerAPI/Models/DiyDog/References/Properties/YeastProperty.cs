using DruidsCornerAPI.Tools;

namespace DruidsCornerAPI.Models.DiyDog.References
{
    /// <summary>
    /// Encodes a known good Yeast property
    /// This is the base for all ReferenceProperties (aka "known good" properties in the DiyDogExtractor databases)
    /// </summary>
    public class YeastProperty : BaseProperty
    {
        /// <summary>
        /// Manufacturer identifier
        /// </summary>
        public string? Manufacturer { get; set; } = null;

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
            var other = obj as YeastProperty;
            bool identical = other != null;
            if(!identical) return false;
            
            identical &= base.Equals(other as YeastProperty);
            if(!identical) return false;

            // Reject non-matching nullity
            identical &= Language.SameNullity(new[] { Manufacturer, other!.Manufacturer });
            identical &= Language.SameNullity(new[] { Aliases, other!.Aliases });
            if(!identical) return false;

            if(Manufacturer != null)
            {
                identical &= Manufacturer == other!.Manufacturer;
            }

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
        public static bool operator == (YeastProperty left, YeastProperty right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Custom inequality operator
        /// </summary>
        public static bool operator != (YeastProperty left, YeastProperty right)
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

            if(Manufacturer != null)
            {
                hash *= Manufacturer.GetHashCode() * 3;
            }
            return hash;
        }
        
    }
}