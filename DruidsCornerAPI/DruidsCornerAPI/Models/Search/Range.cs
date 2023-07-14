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
        /// <example>0</example>
        public T Start { get; set; }

        /// <summary>
        /// Range end value
        /// </summary> 
        /// <example>20</example>
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
        /// Checks if the Range is reversed (Start and End are sorted in a reversed order)
        /// Reverse ordering might be useful for linear interpolation of negative coefficient curves (like NTC thermistors for instance)
        /// </summary>
        public bool IsReversed()
        {
            return Start.CompareTo(End) > 0;
        }

        /// <summary>
        /// Checks if interval span is actually zero (twice the same value for Start and End)
        /// </summary>
        public bool IsNullInterval()
        {
            return Start.CompareTo(End) == 0;
        }
        
        /// <summary>
        /// Swaps Start for End
        /// </summary>
        public void SwapBoundaries()
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

            if(!allowReversed && IsReversed())
            {
                SwapBoundaries();
            }
            Numerics.Clamp(Start, max, min);
            Numerics.Clamp(End, max, min);   
        }

        /// <summary>
        /// Checks that the input value is included in the Range interval 
        /// </summary>
        /// <param name="input">Subject comparison value</param>
        /// <param name="strict">If set to true : Rejects input value that exactly match one of the two boundaries (Strict comparison). Otherwise, lt/gt comparison is performed</param>
        public bool InRange(T input, bool strict = false)
        {
            if(strict)
            {
                return Start.CompareTo(input) < 0 && End.CompareTo(input) > 0;
            }
            return Start.CompareTo(input) <= 0 && End.CompareTo(input) >= 0;
        }
    }
}