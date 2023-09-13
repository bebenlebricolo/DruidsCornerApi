
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
        //
        // /// <summary>
        // /// Tries to read members from the "WebRatingLim" section
        // /// </summary>
        // /// <param name="section"></param>
        // /// <returns></returns>
        // public bool FromConfigSection(IConfigurationSection section)
        // {
        //     JwtSettings = new JwtSettings();
        //     return JwtSettings.FromConfigSection(section.GetSection(JwtSettings.SectionName));
        // }
        //
        // /// <summary>
        // /// Tries to read from the configuration file itself
        // /// </summary>
        // /// <param name="config"></param>
        // /// <returns></returns>
        // public bool FromConfig(IConfiguration config)
        // {
        //     var section = config.GetSection(SectionName);
        //     if (section == null)
        //     {
        //         return false;
        //     }
        //     return FromConfigSection(section);
        // }
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

        // /// <summary>
        // /// Reads from configuration sections
        // /// </summary>
        // /// <param name="section"></param>
        // /// <returns></returns>
        // public bool FromConfigSection(IConfigurationSection section)
        // {
        //     bool success = true;
        //     var validAudiences = section.GetValue<List<string>?>(nameof(ValidAudiences));
        //     var validIssuers = section.GetValue<List<string>?>(nameof(ValidIssuers));
        //
        //     success &= validAudiences != null;
        //     success &= validIssuers != null;
        //
        //     ValidAudiences = validAudiences ?? ValidAudiences; 
        //     ValidIssuers = validIssuers ?? ValidIssuers; 
        //     return success; 
        // }

    }
}
