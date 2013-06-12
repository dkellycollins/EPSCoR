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
