using System.Text.Json.Serialization;

namespace DruidsCornerAPI.Models.Google
{
    public record OAuthAccessToken
    {
        [JsonPropertyName("issued_to")]
        public string issuedTo { get; set; } = "";
        
        [JsonPropertyName("audience")]
        public string audience { get; set; } = "";
        
        [JsonPropertyName("user_id")]
        public string userId { get; set; } = "";
        
        [JsonPropertyName("scope")]
        public string scope { get; set; } = "";
        
        [JsonPropertyName("expires_in")]
        public uint expiresIn { get; set; } = 3600;
        
        [JsonPropertyName("email")]
        public string email { get; set; } = "";
        
        [JsonPropertyName("access_type")]
        public string accessType { get; set; } = "";
        
        [JsonPropertyName("verified_email")]
        public bool verifiedEmail { get; set; } = false;
}
}