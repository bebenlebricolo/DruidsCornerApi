using System.Text.Json.Serialization;

namespace DruidsCornerAPI.Models.Google
{
    public record OAuthAccessToken
    {
        [JsonPropertyName("issued_to")]
        public string issuesTo = "";
        
        [JsonPropertyName("audience")]
        public string audience = "";
        
        [JsonPropertyName("user_id")]
        public string userId = "";
        
        [JsonPropertyName("scope")]
        public string scope = "";
        
        [JsonPropertyName("expires_in")]
        public string expiresIn = "";
        
        [JsonPropertyName("email")]
        public string email = "";
        
        [JsonPropertyName("access_type")]
        public string accessType = "";
        
        [JsonPropertyName("verified_email")]
        public bool verifiedEmail = false;
}
}