using System.Text.Json.Serialization;

namespace DruidsCornerAPI.Models.Google
{
    /// <summary>
    /// Encodes user information as per retrieved from Google's servers
    /// </summary>
    public record UserInfo
    {
        /// <summary>
        /// User email
        /// </summary>
        [JsonPropertyName("email")]
        public string email { get; set; } = "";

        /// <summary>
        /// Family name / surname
        /// </summary>
        [JsonPropertyName("family_name")]
        public string familyName { get; set; } = "";

        /// <summary>
        /// Gender
        /// </summary>
        [JsonPropertyName("gender")]
        public string gender { get; set; } = "";

        /// <summary>
        /// Given name
        /// </summary>
        [JsonPropertyName("given_name")]
        public string givenName { get; set; } = "";

        /// <summary>
        /// Hd
        /// </summary>
        [JsonPropertyName("hd")]
        public string? hd { get; set; } = "";

        /// <summary>
        /// User id
        /// </summary>
        [JsonPropertyName("id")]
        public string id { get; set; } = "";

        /// <summary>
        /// Link to user profile
        /// </summary>
        [JsonPropertyName("link")]
        public string? link { get; set; } = "";

        /// <summary>
        /// User primary name
        /// </summary>
        [JsonPropertyName("name")]
        public string name { get; set; } = "";

        /// <summary>
        /// User's profile picture
        /// </summary>
        [JsonPropertyName("picture")]
        public string picture { get; set; } = "";

        /// <summary>
        /// Whether user email was verified or not. (whether the email is part of the targeted project user base or not?)
        /// </summary>
        [JsonPropertyName("verified_email")]
        public bool verifiedEmail { get; set; } = false;
    }
}