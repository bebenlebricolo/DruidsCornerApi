namespace DruidsCornerAPI.Models.DiyDog.References
{
    /// <summary>
    /// Very simple Known Good Styles list
    /// </summary>
    public class ReferenceStyles
    {
        /// <summary>
        /// List of Style properties
        /// </summary>
        public List<StyleProperty> Styles {get; set; } = new List<StyleProperty>();  

        /// <summary>
        /// Custom comparison operators
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj)
        {
            var other = obj as ReferenceStyles;
            bool identical = other != null;
            identical &= Styles.Count == other!.Styles.Count;
            if(!identical) return false;
            
            // We don't care about the order here, we are looking for the same numbers and nothing more.
            int index = 0;
            while(index < Styles.Count && identical)
            {
                identical &= other.Styles!.Any(item => item == Styles[index]);
                index++;
            }
            return identical;
        }

        /// <summary>
        /// Custom equality operator
        /// </summary>
        public static bool operator == (ReferenceStyles left, ReferenceStyles right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Custom inequality operator
        /// </summary>
        public static bool operator != (ReferenceStyles left, ReferenceStyles right)
        {
            return !left.Equals(right);
        }


        /// <summary>
        /// Custom hasher
        /// </summary>
        public override int GetHashCode()
        {
            var hash = Styles.GetHashCode() * 3;
            return hash;
        }
        
    }
}