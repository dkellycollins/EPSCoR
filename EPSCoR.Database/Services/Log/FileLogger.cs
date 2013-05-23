using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            string logFile = Path.Combine(DirectoryManager.RootDir, "Log.txt");
            if (!File.Exists(logFile))
                File.Create(logFile).Close();
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
    }
}
