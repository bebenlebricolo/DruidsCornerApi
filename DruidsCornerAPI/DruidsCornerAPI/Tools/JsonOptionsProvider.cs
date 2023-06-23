using System.Text.Json.Serialization;
using System.Text.Json;
using System;
using DruidsCornerAPI.Models.DiyDog;

namespace DruidsCornerAPI.Tools
{
    public static class JsonOptionsProvider
    {
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
