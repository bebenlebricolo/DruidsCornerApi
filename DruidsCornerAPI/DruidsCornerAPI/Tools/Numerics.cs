
namespace DruidsCornerAPI.Tools
{
    /// <summary>
    /// Generic Numerics toolsets
    /// </summary>
    public static class Numerics
    {
        /// <summary>
        /// Clamps a generic value within an interval specified by [min,max]
        /// <see href="https://www.codeproject.com/Articles/23323/A-Generic-Clamp-Function-for-C" />
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="max"></param>
        /// <param name="min"></param>
        /// <returns></returns>
        public static T Clamp<T>(T value, T max, T min)
            where T : System.IComparable<T> {
            T result = value;
            if (value.CompareTo(max) > 0)
                result = max;
            if (value.CompareTo(min) < 0)
                result = min;
            return result;
        }
    }
}
