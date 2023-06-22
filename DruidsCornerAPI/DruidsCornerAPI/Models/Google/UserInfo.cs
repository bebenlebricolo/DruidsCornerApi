using System.Text.Json.Serialization;

namespace DruidsCornerAPI.Models.Google
{
    public record UserInfo
    {
        [JsonPropertyName("email")]
        public string email = "";

        [JsonPropertyName("family_name")]
        public string familyName = "";

        [JsonPropertyName("gender")]
        public string gender = "";

        [JsonPropertyName("given_name")]
        public string givenName = "";

        [JsonPropertyName("hd")]
        public string? hd = "";

        [JsonPropertyName("id")]
        public string id = "";

        [JsonPropertyName("link")]
        public string? link = "";

        [JsonPropertyName("name")]
        public string name = "";

        [JsonPropertyName("picture")]
        public string picture = "";

        [JsonPropertyName("verified_email")]
        public bool verifiedEmail = false;
    }
}