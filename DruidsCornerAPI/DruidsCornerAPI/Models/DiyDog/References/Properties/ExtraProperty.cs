using DruidsCornerAPI.Tools;

namespace DruidsCornerAPI.Models.DiyDog.References
{
    public enum ExtraKind
    {
        /// <summary>
        /// Extra ingredient used in the Boil/Fermentation process (in DiyDog recipes, both are merged in the "Hops" category)
        /// </summary>
        Boil,

        /// <summary>
        /// Extra ingredient used in the Mash process
        /// </summary>
        Mash
    }
    
    /// <summary>
    /// Encodes a known good Hop property
    /// This is the base for all ReferenceProperties (aka "known good" properties in the DiyDogExtractor databases)
    /// </summary>
    public class ExtraProperty : BaseProperty
    {
        /// <summary>
        /// Extra ingredient kind, be it a boil/fermentation extra ingredient or a Mash ingredient
        /// </summary>
        /// <value></value>
        public ExtraKind Kind {get; set;} = ExtraKind.Boil;

        /// <summary>
        /// Custom comparison operators
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj)
        {
            var other = obj as ExtraProperty;
            bool identical = other is not null;
            if(!identical) return false;
            
            identical &= base.Equals(other as BaseProperty);
            if(!identical) return false;
            
            return identical;
        }

        /// <summary>
        /// Custom equality operator
        /// </summary>
        public static bool operator == (ExtraProperty? left, ExtraProperty? right)
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
        public static bool operator != (ExtraProperty? left, ExtraProperty? right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Custom hasher
        /// </summary>
        public override int GetHashCode()
        {
            var hash = base.GetHashCode() * 3 + Kind.GetHashCode() * 4;
            return hash;
        }
    }
}