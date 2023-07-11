using System.Linq;

namespace DruidsCornerAPI.Models.Search
{
    /// <summary>
    /// Constructs a multiple query object that'll be used
    /// To search in the database for adequate objects in the system
    /// </summary>
    public record Queries
    {
        /// <summary>
        /// Queried Alcohol By Volume range
        /// </summary>
        public Range<float>? Abv { get; set; }
        
        /// <summary>
        /// Queried International Bitterness Units range
        /// </summary>
        public Range<float>? Ibu { get; set; }
        
        /// <summary>
        /// Queried European Brewery Convention range
        /// </summary>
        public Range<float>? Ebc { get; set; }
        
        /// <summary>
        /// Queried target original gravity range
        /// </summary>
        public Range<float>? TargetOg { get; set; }
        
        /// <summary>
        /// Queried target final gravity range
        /// </summary>
        public Range<float>? TargetFg { get; set; }

        /// <summary>
        /// Queried PH range
        /// </summary>
        public Range<float>? Ph { get; set; }

        /// <summary>
        /// Queried Attenuation Level range
        /// </summary>
        public Range<float>? AttenuationLevel { get; set; }

        /// <summary>
        /// Queried mashing temperature range
        /// </summary>
        public Range<float>? MashTemps { get; set; }

        /// <summary>
        /// Queried fermentation temperature range
        /// </summary>
        public Range<float>? FermentationTemps { get; set; }


        // #########################################################
        // ############## Fuzzy search elements below ############## 
        // #########################################################

        /// <summary>
        /// Queried boil / fermentation elements list (fuzzy search on names)
        /// </summary>
        public List<string>? ExtraBoilList { get; set; }
        
        /// <summary>
        /// Queried extra mash element list (fuzzy search on names1)
        /// </summary>
        public List<string>? ExtraMashList { get; set; }

        /// <summary>
        /// Queried malt list (fuzzy search on names)
        /// </summary>
        public List<string>? MaltList { get; set; }
        
        /// <summary>
        /// Queried hop list (fuzzy search on names)
        /// </summary>
        public List<string>? HopList { get; set; }

        /// <summary>
        /// Queried yeast list (fuzzy search on names)
        /// </summary>
        public List<string>? YeastList { get; set; }

        /// <summary>
        /// Queried twist list (fuzzy search on names)
        /// </summary>
        public List<string>? TwistList {get; set; }

        /// <summary>
        /// Removes doubles from an optional list of string
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private static List<string>? RemoveDoubles(List<string>? list)
        {
            if(list != null)
            {
                list = list.Distinct().ToList();
            }
            return list;
        }

        /// <summary>
        /// Perform pre processing tasks on member values 
        /// in order to sanitize dataset before going deeper in calculations
        /// </summary>
        public void PreProcessParameters()
        {
            Abv?.Sanitize(0.0f, 100.0f);
            Ibu?.Sanitize(0.0f, 100.0f);
            Ebc?.Sanitize(0.0f, 2000.0f);
            TargetOg?.Sanitize(1000.0f, 1200.0f);
            TargetFg?.Sanitize(900.0f, 1050.0f);
            Ph?.Sanitize(3.5f, 8.0f);
            AttenuationLevel?.Sanitize(50.0f, 100.0f);
            MashTemps?.Sanitize(50.0f, 80.0f);
            FermentationTemps?.Sanitize(5.0f, 40.0f);

            // Remove doubles, if any
            RemoveDoubles(ExtraBoilList);
            RemoveDoubles(ExtraMashList);
            RemoveDoubles(MaltList);
            RemoveDoubles(HopList);
            RemoveDoubles(TwistList);
            RemoveDoubles(YeastList);
        }



    }
}