using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EPSCoR.Web.Database.Services;
using EPSCoR.Web.Database.Services.Log;

namespace EPSCoR.Web.FileProcessor
{
    partial class FileProcessorService : ServiceBase
    {
        private static readonly TimeSpan WAIT_TIME = new TimeSpan(0, 1, 0);

        public FileProcessorService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            string rootDir = args[0];
            DirectoryManager.SetRootDirectory(rootDir);

            try
            {
                //Process any files in waiting.
                IEnumerable<string> files = Directory.GetFiles(DirectoryManager.UploadDir);
                foreach (string file in files)
                {
                    FileProcessor.ProcessFileAsync(file);
                }
            }
            catch (Exception ex)
            {
                handleError(ex);
            }

            //Start looking for new files to process.
            fileWatcher.Path = DirectoryManager.UploadDir;
            fileWatcher.EnableRaisingEvents = true;
        }

        protected override void OnStop()
        {
            fileWatcher.EnableRaisingEvents = false;
        }

        private void _fileWatcher_Created(object sender, FileSystemEventArgs e)
        {
            try
            {
                if (File.Exists(e.FullPath))
                {
                    waitUntilFileCanBeOpened(e.FullPath);
                    FileProcessor.ProcessFileAsync(e.FullPath);
                }
            }
            catch (Exception ex)
            {
                handleError(ex);
            }
        }

        private void _fileWatcher_Error(object sender, ErrorEventArgs e)
        {
            handleError(e.GetException());
        }

        private void waitUntilFileCanBeOpened(string filePath)
        {
            bool fileOpened = false;
            while (!fileOpened)
            {
                try
                {
                    using (FileStream fileStream = File.Open(filePath, FileMode.Open))
                    {
                        fileOpened = fileStream.Length > 0;
                    }
                }
                catch
                {
                    FileInfo fileInfo = new FileInfo(filePath);
                    if (DateTime.Now - fileInfo.LastWriteTime > WAIT_TIME)
                    {
                        throw new Exception("Could not open file");
                    }
                }
            }

        }

        private void handleError(Exception e)
        {
            EventLog.WriteEntry("Error: " + e.ToString(), EventLogEntryType.Error);
        }
    }
}
