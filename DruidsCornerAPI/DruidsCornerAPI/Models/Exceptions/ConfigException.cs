namespace DruidsCornerAPI.Models.Exceptions
{
    /// <summary>
    /// Custom configuration exception. Used to state that configuration is messed up.
    /// </summary>
    public class ConfigException : Exception
    {
        /// <summary>
        /// Used when configuration is not properly set
        /// </summary>
        /// <param name="message"></param>
        public ConfigException(string? message) : 
            base(message)
        {
        }
    }
}
