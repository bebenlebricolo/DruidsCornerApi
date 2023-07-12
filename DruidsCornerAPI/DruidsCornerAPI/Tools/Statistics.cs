using System;

namespace DruidsCornerAPI.Tools
{

    /// <summary>
    /// Type constraints for basic numeric type
    /// Inspired from  : https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/generics/constraints-on-type-parameters#constraining-multiple-parameters
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface INumericPod<T> where T : INumericPod<T>
    {
        /// <summary>
        /// Overloading addition operator
        /// </summary>
        public abstract static T operator +(T left, T right);
        
        /// <summary>
        /// Overloading subtraction operator
        /// </summary>
        public abstract static T operator -(T left, T right);
        
        /// <summary>
        /// Overloading product operator
        /// </summary>
        public abstract static T operator *(T left, T right);
    }


    /// <summary>
    /// Performs straight forward statistics operation (probably not the best implementations
    /// you would dream of :) )
    /// </summary>
    public static class Statistics
    {
        /// <summary>
        /// Computes the Geometric mean of an input array
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static double GeometricMean(IList<double> array)
        {
            // Don't process 
            if(array.Count == 0)
            {
                return 0;
            }

            if (array.Count == 1)
            {
                return array[0];
            }

            double product = 1.0;
            foreach(double elem in array)
            {
                product *= elem;
            }

            double result = Math.Pow(product, array.Count);
            return result;
        }

    }
}
