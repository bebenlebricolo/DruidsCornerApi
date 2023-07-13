using System.Text.Json.Serialization;
using System.Text.Json;
using System;
using DruidsCornerAPI.Models.DiyDog.RecipeDb;

namespace DruidsCornerAPI.Tools
{
    /// <summary>
    /// JsonOptionsProvider -> Provides default Json Serialization/Deserizalization to be used whenever using Json objects.
    /// Deals with naming conventions, Enum-String conversions, etc. 
    /// </summary>
    public static class Language
    {
        /// <summary>
        /// Checks that input object array all share the same "Nullity" state
        /// This can be useful for Custom Equality methods as we may want to assert two values are both set to null (which means that they are 
        /// semantically equivalent) or that both values are non null objects and can be effectively compared against one another.
        /// </summary>
        /// <returns>Whether variables share the same nullity state</returns>
        public static bool SameNullity(object?[] objects)
        {
            return SameNullity(new List<object?>(objects));
        }


        /// <summary>
        /// Checks that input object list all share the same "Nullity" state
        /// This can be useful for Custom Equality methods as we may want to assert two values are both set to null (which means that they are 
        /// semantically equivalent) or that both values are non null objects and can be effectively compared against one another.
        /// </summary>
        /// <returns>Whether variables share the same nullity state</returns>
        public static bool SameNullity(List<object?> objects)
        {
            uint nullCounter = 0;
            uint nonNullCounter = 0;

            foreach(var item in objects)
            {
                if(item == null)
                {
                    nullCounter++;
                }
                else
                {
                    nonNullCounter++;
                }
            }

            return (nullCounter == objects.Count)
                || (nonNullCounter == objects.Count);
        }
        
        /// <summary>
        /// Checks that input object can be casted to the adequate type and is valid
        /// This can be useful for Custom Equality methods as we may want to assert two values are both set to null (which means that they are 
        /// semantically equivalent) or that both values are non null objects and can be effectively compared against one another.
        /// </summary>
        /// <returns>Whether variables share the same nullity state</returns>
        public static bool RefCastCheck<T>(object? obj) where T : class
        {
            if(obj is null || obj is not T)
            {
                return false;
            }
            return true;
        }
    }

}
