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

        public FileProcessorService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            string rootDir = args[0];

            fileWatcher.Path = rootDir;
            fileWatcher.EnableRaisingEvents = true;

            DirectoryManager.SetRootDirectory(rootDir);
        }

        protected override void OnStop()
        {
            fileWatcher.EnableRaisingEvents = false;
        }

        private void _fileWatcher_Created(object sender, FileSystemEventArgs e)
        {
            if(File.Exists(e.FullPath))
            {
                FileProcessor.ProcessFileAsync(e.FullPath);
            }
        }

        private void _fileWatcher_Error(object sender, ErrorEventArgs e)
        {
            EventLog.WriteEntry("FILE WATCHER FAILED \n" + e.ToString(), EventLogEntryType.Error);
        }
    }
}
