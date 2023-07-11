using System.Text.Json.Serialization;

namespace DruidsCornerAPI.Models.Google
{
    /// <summary> 
    /// Google's modeled OAuth access token object
    /// Used when retrieving a decoded token with Google's proprietary format.
    /// This one is used when we call google token verification endpoint in order to decode it's response into a datastructure we can work with.
    /// </summary>
    public record OAuthAccessToken
    {
        /// <summary>
        /// Encodes the username/user id to whom that token was delivered.
        /// </summary>
        [JsonPropertyName("issued_to")]
        public string issuedTo { get; set; } = "";
        
        /// <summary>
        /// Encodes the audience of the token
        /// </summary>
        [JsonPropertyName("audience")]
        public string audience { get; set; } = "";
        
        /// <summary>
        /// User id
        /// </summary>
        [JsonPropertyName("user_id")]
        public string userId { get; set; } = "";
        
        /// <summary>
        /// Scopes targeted by this token
        /// </summary>
        [JsonPropertyName("scope")]
        public string scope { get; set; } = "";
        
        /// <summary>
        /// Expiration time
        /// </summary>
        [JsonPropertyName("expires_in")]
        public uint expiresIn { get; set; } = 3600;
        
        /// <summary>
        /// Email of the client to whom this token was delivered.
        /// </summary>
        [JsonPropertyName("email")]
        public string email { get; set; } = "";
        
        /// <summary>
        /// Access type
        /// </summary>
        [JsonPropertyName("access_type")]
        public string accessType { get; set; } = "";
        
        /// <summary>
        /// Tells whether the email was verified (google account/belongs to the targeted project's user base)
        /// </summary>
        [JsonPropertyName("verified_email")]
        public bool verifiedEmail { get; set; } = false;
}
}