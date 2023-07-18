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
        /// Represents a data contract for further filters to implement.
        /// For instance, using this function pointer alias in a method's signature will tell you
        /// that you might be able to use Statistic filters on it (like the GeometricMean.)
        /// It's the closest I found to mimic C function pointer without the burden of C#/C++ OOP with full object instantiation/constraints.
        /// </summary>
        /// <param name="sample"></param>
        /// <returns></returns>    
        public delegate double Filter(IList<double> sample);

        /// <summary>
        /// Computes the Geometric mean of an input sample
        /// </summary>
        /// <param name="sample"></param>
        /// <returns></returns>
        public static double GeometricMean(IList<double> sample)
        {
            // Don't process 
            if(sample.Count == 0)
            {
                return 0;
            }

            if (sample.Count == 1)
            {
                return sample[0];
            }

            uint effectiveCount = 0;
            double product = 1.0;
            foreach(double elem in sample)
            {
                if(elem == 0)
                {
                    // elements with a value of 0 are just 
                    // erasing the other values resulting average. 
                    // Because an alias or name is completely different does not mean we'd like to throw everything away.
                    continue;
                }
                product *= elem;
                // Same remark, effective count is there to only account for non-zero values.
                effectiveCount++;
            }

            double result = Math.Pow(product, (1.0 / effectiveCount));
            return result;
        }

        /// <summary>
        /// Straight forward arithmetic mean of a sample
        /// </summary>
        /// <param name="sample"></param>
        /// <returns></returns>
        public static double ArithmeticMean(IList<double> sample)
        {
            double result = sample.Aggregate((item1, item2) => item1 + item2);
            return result / sample.Count;

        }


    }
}
