using System;

namespace EPSCoR.Database.Services.Log
{
    public class LoggerFactory
    {
        private static ConsoleLogger _consoleLogger;
        private static FileLogger _fileLogger;

        /// <summary>
        /// Statically initalizes the logger.
        /// </summary>
        private static void Init()
        {
            if (_consoleLogger == null)
                _consoleLogger = new ConsoleLogger();
            if (_fileLogger == null)
                _fileLogger = new FileLogger();
        }

        /// <summary>
        /// Writes out the message and exception.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="e"></param>
        public static void Log(string message, Exception e = null)
        {
            Init();
            if (e == null)
            {
                _consoleLogger.Log(message);
                _fileLogger.Log(message);
            }
            else
            {
                _consoleLogger.Log(message, e);
                _fileLogger.Log(message, e);
            }
        }
    }
}
