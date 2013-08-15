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
        private FileSystemWatcher _fileWatcher;

        public FileProcessorService()
        {
            InitializeComponent();

            _fileWatcher = new FileSystemWatcher();
            _fileWatcher.Created += _fileWatcher_Created;
            _fileWatcher.Error += _fileWatcher_Error;
            _fileWatcher.Filter = "*.*";
            _fileWatcher.IncludeSubdirectories = true;
        }

        protected override void OnStart(string[] args)
        {
            string rootDir = args[0];

            _fileWatcher.Path = rootDir;
            _fileWatcher.EnableRaisingEvents = true;

            DirectoryManager.SetRootDirectory(rootDir);
        }

        protected override void OnStop()
        {
            _fileWatcher.EnableRaisingEvents = false;
        }

        private void _fileWatcher_Created(object sender, FileSystemEventArgs e)
        {
            if (File.Exists(e.FullPath)) //This makes sure it was a file that was created.
            {
                //TODO Ensure the file can be opened.
                FileProcessor.ProcessFileAsync(e.FullPath);
            }
        }

        private void _fileWatcher_Error(object sender, ErrorEventArgs e)
        {
            EventLog.WriteEntry("FILE WATCHER FAILED", EventLogEntryType.Error);
        }
    }
}
