using System;

namespace EPSCoR.Database.Services.Log
{
    public class LoggerFactory
    {
        private static ConsoleLogger _consoleLogger;
        private static FileLogger _fileLogger;

        private static void Init()
        {
            if (_consoleLogger == null)
                _consoleLogger = new ConsoleLogger();
            if (_fileLogger == null)
                _fileLogger = new FileLogger();
        }

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
