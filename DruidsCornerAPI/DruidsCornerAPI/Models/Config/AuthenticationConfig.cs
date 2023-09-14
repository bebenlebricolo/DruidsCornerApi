
namespace DruidsCornerAPI.Models.Config
{
    /// <summary>
    /// Stores Web rate limits (used to configure Rest Api rate limits)
    /// </summary>
    public record AuthenticationConfig
    {
        /// <summary>
        /// Section name, used to retrieve the Json object from a local configuration file (usually an appsettings.json file)
        /// </summary>
        public static readonly string SectionName = nameof(AuthenticationConfig);

        /// <summary>
        /// JWT Token authentication settings
        /// </summary>
        public JwtSettings JwtSettings { get; set; } = new JwtSettings();
    }

    /// <summary>
    /// JWT Token authentication settings
    /// </summary>
    public record JwtSettings
    {
        /// <summary>
        /// List of whitelisted Issuers
        /// </summary>
        public List<string> ValidIssuers { get; set; } = new List<string>();
        
        /// <summary>
        /// List of whitelisted audiences
        /// </summary>
        public List<string> ValidAudiences { get; set; } = new List<string>();
    }
}
