using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EPSCoR.Database;
using EPSCoR.Database.Context;
using EPSCoR.Database.Models;
using EPSCoR.Database.Services.FileConverter;
using EPSCoR.Database.Services.Log;

namespace EPSCoR.FileProcessor
{
    public partial class FileProcessingService : ServiceBase
    {
        private static readonly TimeSpan WAIT_TIME = new TimeSpan(0, 1, 0);

        private FileSystemWatcher _fileWatcher;
        private Dictionary<string, Task> _currentTasks;
        private Dictionary<int, CancellationTokenSource> _cancelTokens;
        private bool _deleteFileAfterProcessing;

        public FileProcessingService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _fileWatcher = new FileSystemWatcher(args[0]);
            _fileWatcher.Created += _fileWatcher_Created;
            _fileWatcher.Error += _fileWatcher_Error;

            _fileWatcher.IncludeSubdirectories = true;
            _fileWatcher.Filter = "*.*";
            _fileWatcher.EnableRaisingEvents = true;

            _currentTasks = new Dictionary<string, Task>();
            _cancelTokens = new Dictionary<int, CancellationTokenSource>();

            Boolean.TryParse(args[1], out _deleteFileAfterProcessing);
        }

        protected override void OnStop()
        {
            _fileWatcher.Dispose();
            foreach (CancellationTokenSource cancelTokenSource in _cancelTokens.Values)
            {
                cancelTokenSource.Cancel();
            }
        }

        public Task ProcessFileAsync(string filePath, string tableName = null, string userName = null)
        {
            if (_currentTasks.ContainsKey(filePath))
            {
                return _currentTasks[filePath];
            }

            if (string.IsNullOrEmpty(tableName))
            {
                tableName = Path.GetFileNameWithoutExtension(filePath);
            }
            if (string.IsNullOrEmpty(userName))
            {
                userName = Directory.GetParent(filePath).Name;
            }

            TableIndex tableIndex = new TableIndex()
            {
                Name = tableName,
                Status = "Queued for processing",
                Type = (tableName.Contains("_US")) ? TableTypes.UPSTREAM : TableTypes.ATTRIBUTE,
                UploadedByUser = userName
            };
            using (ModelDbContext context = DbContextFactory.GetModelDbContext())
            {
                context.CreateModel(tableIndex);
            }

            CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
            CancellationToken cancelToken = cancelTokenSource.Token;
            Task task = Task.Factory.StartNew(() => convertFile(filePath, tableIndex, userName, cancelToken), cancelToken, TaskCreationOptions.LongRunning, TaskScheduler.Current);
            task.ContinueWith((t) => cleanUp(t, tableIndex, filePath, userName));

            _currentTasks.Add(filePath, task);
            _cancelTokens.Add(task.Id, cancelTokenSource);
            return task;
        }

        private void _fileWatcher_Created(object sender, FileSystemEventArgs e)
        {
            if (File.Exists(e.FullPath)) //This makes sure it was a file that was created.
            {
                ProcessFileAsync(e.FullPath);
            }
        }

        private void _fileWatcher_Error(object sender, ErrorEventArgs e)
        {
            LoggerFactory.GetLogger().Log("FILE WATCHER FAILED.", e.GetException());
        }

        private void convertFile(string filePath, TableIndex tableIndex, string userName, CancellationToken cancelToken)
        {
            LoggerFactory.GetLogger().Log("File uploaded: " + filePath);
            cancelToken.ThrowIfCancellationRequested();

            //Wait until the file can be opened.
            waitForFile(filePath);

            cancelToken.ThrowIfCancellationRequested();

            //Convert the file.
            updateTableStatus(tableIndex, "Converting uploaded file.");
            string conversionPath = FileConverterFactory.GetConverter(filePath, userName).ConvertToCSV();

            cancelToken.ThrowIfCancellationRequested();

            //Add converted file to the database.
            updateTableStatus(tableIndex, "Creating table in database.");
            using (TableDbContext tableContext = DbContextFactory.GetTableDbContextForUser(userName))
            {
                tableContext.AddTableFromFile(conversionPath);
                tableContext.PopulateTableFromFile(conversionPath);
            }

            //Log when the file was processed.
            LoggerFactory.GetLogger().Log("File processed: " + filePath);
        }

        private void cleanUp(Task task, TableIndex tableIndex, string filePath, string userName)
        {
            _currentTasks.Remove(filePath);
            _cancelTokens.Remove(task.Id);

            if (task.IsFaulted)
            {
                handleError(task, tableIndex, filePath, userName);
            }
            else if (task.IsCanceled)
            {
                handleCancel(task, tableIndex, filePath, userName);
            }
        }

        private void handleError(Task task, TableIndex tableIndex, string filePath, string userName)
        {
            updateTableStatus(tableIndex, "An error occured while processing the file.", false, true);

            LoggerFactory.GetLogger().Log("Exception while processing file: " + filePath, task.Exception);
        }

        private void handleCancel(Task task, TableIndex tableIndex, string filePath, string userName)
        {
            updateTableStatus(tableIndex, "Processing canceled by user.", false, true);

            LoggerFactory.GetLogger().Log("Task canceled while processing: " + filePath);
        }

        public bool canOpenFile(string fileName)
        {
            try
            {
                using (FileStream stream = File.Open(fileName, FileMode.Open))
                {
                    return stream.Length > 0;
                }
            }
            catch
            {
                return false;
            }
        }

        private void waitForFile(string filePath)
        {
            DateTime timeStamp = DateTime.Now;
            long fileSize = 0;
            while (!canOpenFile(filePath))
            {
                FileInfo info = new FileInfo(filePath);
                if (fileSize != info.Length)
                {
                    //The file size has changed, this means this file is still being accessed.
                    fileSize = info.Length;
                    timeStamp = DateTime.Now;
                }
                else
                {
                    //The file size has not changed, check to see how long we have been waiting.
                    if (DateTime.Now - timeStamp > WAIT_TIME) //If we have been trying for too long just stop.
                    {
                        throw new Exception("Could not open file.");
                    }
                }
            }
        }

        /// <summary>
        /// Ensures that File.Move and File.Copy will not throw an exception becuase a directory does not exist or a file of the same name does.
        /// </summary>
        /// <param name="dest">The destination path.</param>
        private void validateDestination(string dest)
        {
            string destDir = Directory.GetParent(dest).FullName;
            if (!Directory.Exists(destDir))
                Directory.CreateDirectory(destDir);
            if (File.Exists(dest))
                File.Delete(dest);
        }

        private void updateTableStatus(TableIndex index, string status, bool processed = false, bool error = false)
        {
            index.Status = status;
            index.Processed = processed;
            index.Error = error;
            using (ModelDbContext context = DbContextFactory.GetModelDbContext())
            {
                context.UpdateModel(index);
            }
        }
    }
}
