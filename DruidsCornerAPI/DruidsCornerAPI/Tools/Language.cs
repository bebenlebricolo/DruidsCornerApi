using System.Text.Json.Serialization;
using System.Text.Json;
using System;
using DruidsCornerAPI.Models.DiyDog.RecipeDb;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;
using System.Numerics;
using System.Collections;

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
        
        /// <summary>
        /// Compares two lists and checks that both lists contain equivalent data with or without order requirement
        /// Uses the regular Equality operator == : will compare references if not specifically overloaded to compare object's value instead! 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static bool CompareEquivalentLists<T>(List<T>? list1, List<T>? list2, bool orderSensitive = false)
        {
            
            // We know that both lists are null (thanks to the above line)
            if(!SameNullity(new [] {list1, list2}))
            {
                return false;
            }

            // Both are null !
            if(list1 == null)
            {
                return true;
            }

            // element count shall match as well
            if(list1.Count != list2!.Count)
            {
                return false;
            }

            // I know some stuff might be null from the typing system's point of view
            // But here there's nothing null at this point anymore
            #pragma warning disable CS8602
            bool identical = true;
            int index = 0;
            if(orderSensitive)
            {
                while(index < list1!.Count && identical)
                {
                    identical &= list1![index].Equals(list2![index]);
                    index++;
                }    
            }
            else
            {
                while(index < list1!.Count && identical)
                {
                    identical &= list2!.Any(item => item.Equals(list2![index]));
                    index++;
                }
            }

            return identical;
        }
    }

}
