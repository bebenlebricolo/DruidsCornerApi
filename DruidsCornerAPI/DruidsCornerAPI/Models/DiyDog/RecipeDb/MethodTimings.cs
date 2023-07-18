using DruidsCornerAPI.Tools;

namespace DruidsCornerAPI.Models.DiyDog.RecipeDb
{
    /// <summary>
    /// Method Timings characteristics
    /// </summary>
    public class MethodTimings
    {
        /// <summary>
        /// Mashing temperatures
        /// </summary>
        public List<MashTemp> MashTemps { get; set; } = new List<MashTemp>();

        /// <summary>
        /// Mashing tips
        /// </summary>
        public List<string> MashTips { get; set; } = new List<string>();

        /// <summary>
        /// Fermentation instructions
        /// </summary>
        public Fermentation Fermentation { get; set; } = new Fermentation();

        /// <summary>
        /// Recipe twists (custom ingredients)
        /// </summary>
        public List<Twist>? Twists { get; set; } = new List<Twist>();

        /// <summary>
        /// Custom comparison operators
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj)
        {
            if(obj is not MethodTimings)  
            {
                return false;
            }
            return Equals(obj as MethodTimings);
        }

        /// <summary>
        /// Custom comparison operators
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(MethodTimings? other)
        {  
            bool identical = other is not null;
            identical &= Language.CompareEquivalentLists(MashTemps, other!.MashTemps);
            identical &= Language.CompareEquivalentLists(MashTips, other!.MashTips);
            identical &= Language.CompareEquivalentLists(Twists, other!.Twists);
            identical &= Fermentation == other!.Fermentation;

            return identical;
        }

        /// <summary>
        /// Customc equality operator
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator==(MethodTimings? left, MethodTimings? right)
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
        /// Customc inequality operator
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator!=(MethodTimings? left, MethodTimings? right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Custom hasher
        /// </summary>
        public override int GetHashCode()
        {
            var hash = MashTemps.GetHashCode() * 3;
            hash += MashTips.GetHashCode() * 3;
            hash += Fermentation.GetHashCode() * 3;
            hash += Twists.GetHashCode() * 3;
            return hash;
        }


    }
}
