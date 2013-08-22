using System;
using System.IO;

namespace EPSCoR.Web.Database.Services.Log
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
                    new FileLogger(Path.Combine(DirectoryManager.RootDir, "logs")),
                    new DbLogger()
                    );
            }
            return _loggerInstance;   
        }
    }
}
