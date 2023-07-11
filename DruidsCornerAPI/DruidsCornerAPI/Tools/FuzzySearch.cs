using System.Text.Json.Serialization;
using System.Text.Json;
using System;
using DruidsCornerAPI.Models.DiyDog;
using FuzzySharp;

namespace DruidsCornerAPI.Tools
{

    /// <summary>
    /// Template delegate accessor definition to be used in conjunction with FuzzySearch static class
    /// <see cref="FuzzySearch"/>
    /// </summary>
    /// <param name="elem">Any object</param>
    /// <returns>string</returns>
    public delegate string GetProp<T>(T elem); 

    /// <summary>
    /// FuzzySearch toolset 
    /// </summary>
    public static class FuzzySearch
    {
      
        /// <summary>
        /// Searches for a match with words individual matching using Fuzzy search algorithms
        /// </summary>
        /// <param name="property">Input property string we'll try to match against a known reference list.</param>
        /// <param name="list">List of input objects against which we'll compare input property.</param>
        /// <param name="propAccessor">A delegate function that allows this method to access to targeted property (such as T.Name). <see cref="GetProp{T}"/></param>
        /// <returns>Tuple of MaxRatio (0-100) and the reference object that has the highest probability</returns>
        /// <throws>NullReferenceException if input list is empty</throws>
        public static Tuple<int, T> SearchPartialRatio<T>(string property, List<T> list, GetProp<T> propAccessor )
        {
            int maxRatio = 0;

            // Will throw a null reference exception if list does not contain anything
            T? mostProbableElem = list.First();
            foreach (var element in list) 
            {
                var objectProp = propAccessor.Invoke(element);

                var ratio = Fuzz.PartialRatio(property, objectProp);
                if(ratio > maxRatio)
                {
                    maxRatio = ratio;
                    mostProbableElem = element;
                }

                // Found an exact match
                if(ratio == 100)
                {
                    break;
                }
            }

            return new Tuple<int, T>(maxRatio, mostProbableElem!);
        }
    
        /// <summary>
        /// Variation of SearchExactRatio <see cref="SearchPartialRatio"/> that returns a full list of ratios, reversed order 
        /// with Highest probability element as first element and least probable one as last element
        /// </summary>
        /// <param name="property"></param>
        /// <param name="list"></param>
        /// <param name="propAccessor"></param>
        /// <returns></returns>
        public static List<Tuple<int, T>> SearchPartialRatioCompleteList<T>(string property, List<T> list, GetProp<T> propAccessor )
        {
            var outList = new List<Tuple<int, T>>();
            
            foreach (var element in list) 
            {
                var objectProp = propAccessor.Invoke(element);

                var ratio = Fuzz.Ratio(property, objectProp);
                outList.Add(new Tuple<int, T>(ratio,element));
            }

            outList.Sort((elem1, elem2) => elem2.Item1.CompareTo(elem1.Item1));

            return outList;
        }
    
    
    }
}
