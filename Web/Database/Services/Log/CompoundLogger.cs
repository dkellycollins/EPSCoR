using System;

namespace EPSCoR.Web.Database.Services.Log
{
    public class CompoundLogger : ILogger
    {
        private ILogger[] _loggers;

        public CompoundLogger(params ILogger[] loggers)
        {
            _loggers = loggers;
        }

        public void Log(string message)
        {
            foreach (ILogger logger in _loggers)
                logger.Log(message);
        }

        public void Log(string message, Exception e)
        {
            foreach (ILogger logger in _loggers)
                logger.Log(message, e);
        }
    }
}
