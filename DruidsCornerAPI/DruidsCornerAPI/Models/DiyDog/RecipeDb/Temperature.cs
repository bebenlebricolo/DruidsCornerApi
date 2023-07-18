using DruidsCornerAPI.Tools;

namespace DruidsCornerAPI.Models.DiyDog.RecipeDb
{
    /// <summary>
    /// Temperature instruction for Mash
    /// </summary>
    public class Temperature
    {
        /// <summary>
        /// Temperature expressed in Celsius degrees
        /// </summary>
        public float Celsius { get; set; } = 0.0f;

        /// <summary>
        /// Temperature expressed using Fahrenheit degrees
        /// </summary>
        public float Fahrenheit { get; set; } = 0.0f;

        /// <summary>
        /// Custom comparison operators
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj)
        {
            if(obj is not Temperature)  
            {
                return false;
            }
            return Equals(obj as Temperature);
        }

        /// <summary>
        /// Custom comparison operators
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Temperature? other)
        {  
            bool identical = other is not null;
            
            // We don't care about the ordering here
            identical &= Celsius == other!.Celsius;
            identical &= Fahrenheit == other!.Fahrenheit;
            return identical;
        }

        /// <summary>
        /// Custom equality operator
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator==(Temperature? left, Temperature? right)
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
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator!=(Temperature? left, Temperature? right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Custom hasher
        /// </summary>
        public override int GetHashCode()
        {
            var hash = Celsius.GetHashCode() * 2 + Fahrenheit.GetHashCode() * 3;
            return hash;
        }
    }

    /// <summary>
    /// Mash temperature
    /// </summary>
    public class MashTemp : Temperature
    {
        /// <summary>
        /// Time of the mash
        /// </summary>
        public float Time { get; set; } = 0.0f;
    }
}
