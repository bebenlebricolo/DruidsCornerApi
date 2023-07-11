using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DruidsCornerAPI.Tools.Logging
{

    /// <summary>
    /// Application Logging static toolsets
    /// Borrowed from here <see aref="https://stackify.com/net-core-loggerfactory-use-correctly/"/>
    /// </summary>
    public static class ApplicationLogging
    {
        private static ILoggerFactory? _Factory = null;
        
        /// <summary>
        /// Instantiate a LoggerFactory if need be
        /// </summary>
        public static ILoggerFactory LoggerFactory
        {
            get
            {
                if (_Factory == null)
                {
                    _Factory = new LoggerFactory();
                }
                return _Factory;
            }
            set { _Factory = value; }
        }

        /// <summary>
        /// Instantiates a Logger using the provided type.
        /// This is useful for classes that does not inherit from the MVC infrastructure.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static ILogger<T> CreateLogger<T>() => LoggerFactory.CreateLogger<T>();
    }    
}