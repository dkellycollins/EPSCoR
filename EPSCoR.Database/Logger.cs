using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPSCoR.Database
{
    /// <summary>
    /// Handles logging messages.
    /// </summary>
    internal class Logger
    {


        /// <summary>
        /// Appends the entry to the log file with the current time.
        /// </summary>
        /// <param name="entry">Log entry.</param>
        public static void Log(string entry)
        {
            string logFile = Path.Combine(DirectoryManager.RootDir, "Log.txt");
            StreamWriter logFileStream;
            if (!File.Exists(logFile))
                logFileStream = File.CreateText(logFile);
            else
                logFileStream = new StreamWriter(File.Open(logFile, FileMode.Append));

            logFileStream.WriteLine(string.Format("{0} - {1}", DateTime.Now.ToString(), entry));

            logFileStream.Flush();
            logFileStream.Close();
        }
    }
}
