using DruidsCornerAPI.Tools;

namespace DruidsCornerAPI.Models.Search
{
    /// <summary>
    /// Range with numeric values
    /// </summary>
    public record Range<T> where T : System.IComparable<T>
    {
        /// <summary>
        /// Range start value
        /// </summary>
        public T Start { get; set; }

        /// <summary>
        /// Range end value
        /// </summary> 
        public T End { get; set; }

        /// <summary>
        /// Standard range constructor
        /// </summary>
        /// <param name="start">Range start</param>
        /// <param name="end">Range end</param>
        public Range(T start, T end)
        {
            Start = start;
            End = end;
        }

        /// <summary>
        /// Swaps Start for End
        /// </summary>
        public void Reverse()
        {
            var tmp = Start;
            Start = End;
            End = tmp;
        }

        /// <summary>
        /// Clamp Start and End values to well known interval [min,max]
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public void Clamp(T min, T max)
        {
            Numerics.Clamp(Start, max, min);
        }

        /// <summary>
        /// Reverses and clamps range
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="allowReversed">If set to true, this will allow Start and End values to be swapped around. This is useful when interpolating positive monotonic curve into a negative monotonic curve</param>
        public void Sanitize(T min, T max, bool allowReversed = true)
        {
            // Swap input for min to always be lower than max
            if(min.CompareTo(max) > 0)
            {
                var tmp = min;
                min = max;
                max = tmp;
            }

            if(!allowReversed && Start.CompareTo(End) > 0)
            {
                Reverse();
            }
            Numerics.Clamp(Start, max, min);
            Numerics.Clamp(End, max, min);   
        }
    }
}