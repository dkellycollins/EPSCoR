using System;

namespace EPSCoR.Database.Services.Log
{
    public class LoggerFactory
    {
        private static ILogger _loggerInstance;

        /// <summary>
        /// Creates and returns an instance of an ILogger object.
        /// </summary>
        /// <returns></returns>
        public static ILogger GetLogger()
        {
            if (_loggerInstance == null)
            {
                _loggerInstance = new CompoundLogger(
                    new FileLogger(),
                    new EmailLogger(
                        "dkellycollins@gmail.com"
                        )
                    );
            }
            return _loggerInstance;
        }
    }
}
