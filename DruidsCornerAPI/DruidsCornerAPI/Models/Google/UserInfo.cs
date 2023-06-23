using System.Text.Json.Serialization;

namespace DruidsCornerAPI.Models.Google
{
    public record UserInfo
    {
        [JsonPropertyName("email")]
        public string email { get; set; } = "";

        [JsonPropertyName("family_name")]
        public string familyName { get; set; } = "";

        [JsonPropertyName("gender")]
        public string gender { get; set; } = "";

        [JsonPropertyName("given_name")]
        public string givenName { get; set; } = "";

        [JsonPropertyName("hd")]
        public string? hd { get; set; } = "";

        [JsonPropertyName("id")]
        public string id { get; set; } = "";

        [JsonPropertyName("link")]
        public string? link { get; set; } = "";

        [JsonPropertyName("name")]
        public string name { get; set; } = "";

        [JsonPropertyName("picture")]
        public string picture { get; set; } = "";

        [JsonPropertyName("verified_email")]
        public bool verifiedEmail { get; set; } = false;
    }
}