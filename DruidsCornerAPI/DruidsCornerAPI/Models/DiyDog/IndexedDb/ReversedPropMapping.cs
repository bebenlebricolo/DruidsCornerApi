using DruidsCornerAPI.Tools;

namespace DruidsCornerAPI.Models.DiyDog.IndexedDb
{

    /// <summary>
    /// Encodes a simple reversed property mapping as found in {prop}_rv_db.json provided by 
    /// DiyDogExtract databases
    /// </summary>
    public class ReversedPropMapping
    {
        /// <summary>
        /// Property name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// List of recipes ordered by their number / id in which this property was found.
        /// </summary>
        public List<uint> FoundInBeers { get; set; } = new List<uint>();

        /// <summary>
        /// Custom comparison operators
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj)
        {
            var other = obj as ReversedPropMapping;
            bool identical = other != null;
            if(!identical) return false;
                      
            identical &= Name == other!.Name;
            if(!identical) return false;

            identical &= FoundInBeers.Count == other.FoundInBeers.Count;
            if(!identical) return false;

            // We don't care about the order here, we are looking for the same numbers and nothing more.
            int index = 0;
            while(index < FoundInBeers.Count && identical)
            {
                identical &= other.FoundInBeers.Contains(FoundInBeers[index]);
                index++;
            }

            return identical;
        }

        /// <summary>
        /// Custom hasher
        /// </summary>
        public override int GetHashCode()
        {
            return Name.GetHashCode() * 32 + FoundInBeers.GetHashCode() * 11;
        }
    }

}