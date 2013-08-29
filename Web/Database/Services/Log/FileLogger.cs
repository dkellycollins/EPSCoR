using System;
using System.IO;

namespace EPSCoR.Web.Database.Services.Log
{
    /// <summary>
    /// Writes messages to a file on the database.
    /// </summary>
    public class FileLogger : ILogger
    {
        private object _lock;
        private string _directory;

        public FileLogger(string directory)
        {
            _lock = new object();
            _directory = directory;
        }

        /// <summary>
        /// Appends the entry to the log file with the current time.
        /// </summary>
        /// <param name="entry">Log entry.</param>
        public void Log(string entry)
        {
            string logFile = getLogFilePath();
            lock (_lock)
            {
                using (StreamWriter logFileStream = new StreamWriter(File.Open(logFile, FileMode.OpenOrCreate)))
                {
                    logFileStream.BaseStream.Seek(0, SeekOrigin.End); //Seek to end of the file.
                    logFileStream.WriteLine(string.Format("{0} - {1}", DateTime.Now.ToString(), entry));
                    logFileStream.Flush();
                }
            }
        }

        /// <summary>
        /// Appends the entry to the log with the current time. Also will write out the error message and stack trace for the exception. If the exception has an inner exception will write out that as well.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="e"></param>
        public void Log(string message, Exception e)
        {
            string logFile = getLogFilePath();
            lock (_lock)
            {
                using (StreamWriter logFileStream = new StreamWriter(File.Open(logFile, FileMode.Append)))
                {
                    logFileStream.BaseStream.Seek(0, SeekOrigin.End); //Seek to end of the file.
                    logFileStream.WriteLine(string.Format("{0} - {1}", DateTime.Now.ToString(), message));

                    Exception currentException = e;
                    while (currentException != null)
                    {
                        logFileStream.WriteLine(e.Message);
                        logFileStream.WriteLine(e.StackTrace);
                        currentException = currentException.InnerException;
                    }

                    logFileStream.Flush();
                }
            }
        }

        private string getLogFilePath()
        {
            return Path.Combine(_directory, "Logs", "Log-" + DateTime.Now.ToShortDateString() + ".txt");
        }
    }
}
