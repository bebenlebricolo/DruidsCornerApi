namespace DruidsCornerAPI.Models.Exceptions
{
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
