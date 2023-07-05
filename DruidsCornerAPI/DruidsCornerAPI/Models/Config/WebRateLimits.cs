
namespace DruidsCornerAPI.Models.Config
{
    public record WebRateLimits
    {
        public static readonly string SectionName = nameof(WebRateLimits);

        /// <summary>
        /// Specifies the configured Rate limit per time frame.
        /// </summary>
        public int PermitLimit { get; set; } = 4;

        /// <summary>
        /// Specifies the time window size for Rate Limiting
        /// </summary>
        public int WindowSeconds { get; set; } = 12;

        public int SegmentsPerWindow {get; set;} = 6;
        public int QueueLimit {get; set;} = 3;

        /// <summary>
        /// Tries to read members from the "WebRatingLim" section
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        public bool FromConfigSection(IConfigurationSection section)
        {
            bool success = true;
            var permitLimit = section.GetValue<int?>(nameof(PermitLimit));
            var windowSeconds = section.GetValue<int?>(nameof(WindowSeconds));
            var segmentsPerWindow = section.GetValue<int?>(nameof(SegmentsPerWindow));
            var queueLimit = section.GetValue<int?>(nameof(QueueLimit));

            success &= permitLimit != null;
            success &= windowSeconds != null;
            success &= segmentsPerWindow != null;
            success &= queueLimit != null;

            PermitLimit = permitLimit ?? PermitLimit; 
            WindowSeconds = windowSeconds ?? WindowSeconds;
            SegmentsPerWindow = segmentsPerWindow ?? SegmentsPerWindow;
            QueueLimit = queueLimit ?? QueueLimit;
            return success; 
        }

        /// <summary>
        /// Tries to read from the configuration file itself
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public bool FromConfig(IConfiguration config)
        {
            var section = config.GetSection(SectionName);
            if (section == null)
            {
                return false;
            }
            return FromConfigSection(section);
        }
    }
}
