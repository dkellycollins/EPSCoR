using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EPSCoR.Database.Context;
using EPSCoR.Database.Models;
using EPSCoR.Database.Services;
using EPSCoR.Database.Services.FileConverter;
using EPSCoR.Database.Services.Log;
using System.Linq;
using EPSCoR.Util;

namespace EPSCoR.Database
{
    /// <summary>
    /// The file processor coverts any uploaded files into csv file and moves them to the archive folder.
    /// </summary>
    /// <remarks>This needs to be refactored to run as an independent application.</remarks>
    public class FileProcessor
    {
        private static TimeSpan WAIT_TIME = new TimeSpan(0, 5, 0);

        private FileSystemWatcher _fileWatcher;
        private FilePoll _filePoll;
        private Dictionary<string, Task> _currentTasks;
        private Dictionary<int, CancellationTokenSource> _cancelTokens;

        public ReadOnlyDictionary<string, Task> CurrentTasks { get; private set; }
        public ReadOnlyDictionary<int, CancellationTokenSource> CancelTokens { get; private set; }

        #region Public Members

        public FileProcessor(string dataDirectory)
        {
            DirectoryManager.SetRootDirectory(dataDirectory);

            _fileWatcher = new FileSystemWatcher(DirectoryManager.UploadDir);
            _fileWatcher.Created += _fileWatcher_Created;
            _fileWatcher.Error += _fileWatcher_Error;

            _fileWatcher.IncludeSubdirectories = true;
            _fileWatcher.Filter = "*.*";
            _fileWatcher.EnableRaisingEvents = true;

            //_filePoll = new FilePoll(10000, DirectoryManager.RootDir);
            //_filePoll.FileFound += _filePoll_FileFound;

            _currentTasks = new Dictionary<string, Task>();
            _cancelTokens = new Dictionary<int, CancellationTokenSource>();
            CurrentTasks = new ReadOnlyDictionary<string, Task>(_currentTasks);
            CancelTokens = new ReadOnlyDictionary<int, CancellationTokenSource>(_cancelTokens);
        }

        public void Dispose()
        {
            _fileWatcher.Dispose();
            foreach (CancellationTokenSource cancelTokenSource in _cancelTokens.Values)
            {
                cancelTokenSource.Cancel();
            }
        }

        public Task ProcessFileAsync(string filePath)
        {
            if (_currentTasks.ContainsKey(filePath))
            {
                return _currentTasks[filePath];
            }

            //TODO improve this section.s
            //Wait until the file can be opened.
            DateTime timeStamp = DateTime.Now;
            while (!IsFileReady(filePath))
            {
                if (DateTime.Now - timeStamp > WAIT_TIME) //If we have been trying for too long just stop.
                {
                    throw new Exception("Could not open file.");
                }
            }

            string fileKey = FileKeyGenerator.GenerateKey(filePath);
            TableIndex tableIndex = null;
            using (ModelDbContext context = DbContextFactory.GetModelDbContext())
            {
                tableIndex = context.GetAllModels<TableIndex>().Where((i) => i.FileKey == fileKey).FirstOrDefault();
            }

            if (tableIndex == null)
            {
                LoggerFactory.GetLogger().Log("Could not find table index for " + filePath);
                return null;
            }

            CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
            CancellationToken cancelToken = cancelTokenSource.Token;
            Task task = Task.Factory.StartNew(() => convertFile(filePath, tableIndex, cancelToken), cancelToken, TaskCreationOptions.LongRunning, TaskScheduler.Current);
            task.ContinueWith((t) => cleanUp(t, tableIndex, filePath));

            _currentTasks.Add(filePath, task);
            _cancelTokens.Add(task.Id, cancelTokenSource);
            return task;
        }

        #endregion Public Members

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

        private void _filePoll_FileFound(string filePath)
        {
            ProcessFileAsync(filePath);
        }

        private void convertFile(string filePath, TableIndex tableIndex, CancellationToken cancelToken)
        {
            LoggerFactory.GetLogger().Log("File uploaded: " + filePath);

            using (ModelDbContext defaultContext = DbContextFactory.GetModelDbContext())
            {
                cancelToken.ThrowIfCancellationRequested();

                //Convert the file.
                updateTableStatus(defaultContext, tableIndex, "Converting uploaded file.");
                string conversionPath = FileConverterFactory.GetConverter(filePath, tableIndex.UploadedByUser).ConvertToCSV();

                cancelToken.ThrowIfCancellationRequested();

                //Add converted file to the database.
                tableIndex.FileKey = FileKeyGenerator.GenerateKey(conversionPath);
                updateTableStatus(defaultContext, tableIndex, "Creating table in database.");
                using (TableDbContext tableContext = DbContextFactory.GetTableDbContextForUser(tableIndex.UploadedByUser))
                {
                    tableContext.AddTableFromFile(conversionPath);
                    tableContext.PopulateTableFromFile(conversionPath);
                }

                //Move the original file to the Archive.
                updateTableStatus(defaultContext, tableIndex, "Table created.", true);
                string archivePath = Path.Combine(DirectoryManager.ArchiveDir, tableIndex.UploadedByUser, Path.GetFileName(filePath));
                validateDestination(archivePath);
                File.Move(filePath, archivePath);

                //Log when the file was processed.
                LoggerFactory.GetLogger().Log("File processed: " + filePath);
            }
        }

        private void cleanUp(Task task, TableIndex tableIndex, string filePath)
        {
            _currentTasks.Remove(filePath);
            _cancelTokens.Remove(task.Id);

            if (task.IsFaulted)
            {
                handleError(task, tableIndex, filePath);
            }
            else if (task.IsCanceled)
            {
                handleCancel(task, tableIndex, filePath);
            }
        }

        private void handleError(Task task, TableIndex tableIndex, string filePath)
        {
            using (ModelDbContext defaultContext = new MySqlModelDbContext())
            {
                updateTableStatus(defaultContext, tableIndex, "An error occured while processing the file.", false, true);
            }

            LoggerFactory.GetLogger().Log("Exception while processing file: " + filePath, task.Exception);

            //Move the invalid file.
            try
            {
                string invalidPath = Path.Combine(DirectoryManager.InvalidDir, tableIndex.UploadedByUser, Path.GetFileName(filePath));
                validateDestination(invalidPath);
                File.Move(filePath, invalidPath);
            }
            catch { }
        }

        private void handleCancel(Task task, TableIndex tableIndex, string filePath)
        {
            using (ModelDbContext defaultContext = new MySqlModelDbContext())
            {
                updateTableStatus(defaultContext, tableIndex, "Processing canceled by user.", false, true);
            }

            LoggerFactory.GetLogger().Log("Task canceled while processing: " + filePath);
        }

        public bool IsFileReady(string fileName)
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

        private void updateTableStatus(ModelDbContext context, TableIndex index, string status, bool processed = false, bool error = false)
        {
            index.Status = status;
            index.Processed = processed;
            index.Error = error;
            context.UpdateModel(index);
        }
    }
}
