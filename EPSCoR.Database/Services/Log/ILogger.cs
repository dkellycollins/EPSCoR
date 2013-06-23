using System;

namespace EPSCoR.Database.Services.Log
{
    public interface ILogger
    {
        /// <summary>
        /// Writes out the message.
        /// </summary>
        /// <param name="message"></param>
        void Log(string message);

        /// <summary>
        /// Writes out the message and the message and stack trace for the exception. If the exception has an inner exception will write that out as well.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="e"></param>
        void Log(string message, Exception e);
    }
}
