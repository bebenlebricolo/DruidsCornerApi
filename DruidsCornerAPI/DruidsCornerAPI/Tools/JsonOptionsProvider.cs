using System.Text.Json.Serialization;
using System.Text.Json;
using System;
using DruidsCornerAPI.Models.DiyDog;

namespace DruidsCornerAPI.Tools
{
    /// <summary>
    /// JsonOptionsProvider -> Provides default Json Serialization/Deserizalization to be used whenever using Json objects.
    /// Deals with naming conventions, Enum-String conversions, etc. 
    /// </summary>
    public static class JsonOptionsProvider
    {
        /// <summary>
        /// Generates default Json serializer options
        /// </summary>
        /// <returns></returns>
        public static JsonSerializerOptions GetModelsJsonOptions()
        {
            var options = new JsonSerializerOptions {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters =
                {
                    new JsonStringEnumConverter(),
                    new DataRecordPolymorphicConverter()
                }
            };

            return options;
        }
    }
}
