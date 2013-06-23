using System;
using System.IO;

namespace EPSCoR.Database.Services.Log
{
    /// <summary>
    /// Handles logging messages.
    /// </summary>
    public class FileLogger : ILogger
    {
        private string _logFile;

        public FileLogger()
        {
            _logFile = Path.Combine(DirectoryManager.RootDir, "Log.txt");
            if (!File.Exists(_logFile))
                File.Create(_logFile).Close();
        }

        /// <summary>
        /// Appends the entry to the log file with the current time.
        /// </summary>
        /// <param name="entry">Log entry.</param>
        public void Log(string entry)
        {
            StreamWriter logFileStream = new StreamWriter(File.Open(_logFile, FileMode.Append));

            logFileStream.WriteLine(string.Format("{0} - {1}", DateTime.Now.ToString(), entry));

            logFileStream.Flush();
            logFileStream.Close();
        }

        /// <summary>
        /// Appends the entry to the log with the current time. Also will write out the error message and stack trace for the exception. If the exception has an inner exception will write out that as well.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="e"></param>
        public void Log(string message, Exception e)
        {
            StreamWriter logFileStream = new StreamWriter(File.Open(_logFile, FileMode.Append));

            logFileStream.WriteLine(string.Format("{0} - {1}", DateTime.Now.ToString(), message));
            
            Exception currentException = e;
            while (currentException != null)
            {
                logFileStream.WriteLine(e.Message);
                logFileStream.WriteLine(e.StackTrace);
                currentException = currentException.InnerException;
            }

            logFileStream.Flush();
            logFileStream.Close();
        }
    }
}
