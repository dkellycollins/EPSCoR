﻿using System;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using EPSCoR.Database.Exceptions;
using EPSCoR.Database.Models;
using EPSCoR.Database.Services;
using EPSCoR.Database.Services.FileConverter;
using EPSCoR.Database.Services.Log;

namespace EPSCoR.Database
{
    /// <summary>
    /// The file processor coverts any uploaded files into csv file and moves them to the archive folder.
    /// </summary>
    /// <remarks>This needs to be refactored to run as an independent application.</remarks>
    public class FileProcessor
    {
        static FileProcessor()
        {
            DirectoryManager.Initialize(HttpContext.Current.Server);
        }

        private static TimeSpan WAIT_TIME = new TimeSpan(0, 1, 0);
        private FileSystemWatcher _fileWatcher;

        public FileProcessor()
        {
            _fileWatcher = new FileSystemWatcher(DirectoryManager.UploadDir);
            _fileWatcher.Created += _fileWatcher_Created;
            //_fileWatcher.Changed += _fileWatcher_Changed;
            _fileWatcher.Error += _fileWatcher_Error;

            _fileWatcher.IncludeSubdirectories = true;
            _fileWatcher.EnableRaisingEvents = true;
        }

        public void Dispose()
        {
            _fileWatcher.Dispose();
        }

        private void _fileWatcher_Created(object sender, FileSystemEventArgs e)
        {
            convertFile(e.FullPath);
        }

        private void _fileWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Changed || e.ChangeType == WatcherChangeTypes.Created)
                Task.Factory.StartNew(() => convertFile(e.FullPath));
        }

        private void _fileWatcher_Error(object sender, ErrorEventArgs e)
        {
            LoggerFactory.GetLogger().Log("FILE WATCHER FAILED.", e.GetException());
        }

        private static void convertFile(string file)
        {
            LoggerFactory.GetLogger().Log("File uploaded:" + file);

            using (DefaultContext defaultContext = new DefaultContext())
            {
                //Create table entry.
                string tableName = Path.GetFileNameWithoutExtension(file);
                string userName = Directory.GetParent(file).Name;
                TableIndex tableIndex = new TableIndex()
                {
                    Name = tableName,
                    DateCreated = DateTime.Now,
                    DateUpdated = DateTime.Now,
                    Status = "Queued for processing",
                    Type = (tableName.Contains("_US")) ? TableTypes.UPSTREAM : TableTypes.ATTRIBUTE,
                    UploadedByUser = userName
                };
                defaultContext.Tables.Add(tableIndex);
                defaultContext.SaveChanges();

                try
                {
                    //Wait until the file can be opened.
                    DateTime timeStamp = DateTime.Now;
                    while (!IsFileReady(file))
                    {
                        if (DateTime.Now - timeStamp > WAIT_TIME) //If we have been trying for too long just stop.
                        {
                            throw new Exception("Could not open file.");
                        }
                    }

                    //Convert the file.
                    updateTableStatus(defaultContext, tableIndex, "Converting uploaded file.");
                    string conversionPath = FileConverterFactory.GetConverter(file, userName).ConvertToCSV();

                    //Add converted file to the database.
                    updateTableStatus(defaultContext, tableIndex, "Creating table in database.");
                    using (UserContext userContext = UserContext.GetContextForUser(userName))
                    {
                        userContext.Procedures.AddTableFromFile(conversionPath);
                        userContext.Procedures.PopulateTableFromFile(conversionPath);
                    }

                    //Move the original file to the Archive.
                    updateTableStatus(defaultContext, tableIndex, "Table created.", true);
                    string archivePath = Path.Combine(DirectoryManager.ArchiveDir, userName, Path.GetFileName(file));
                    validateDestination(archivePath);
                    File.Move(file, archivePath);

                    //Log when the file was processed.
                    LoggerFactory.GetLogger().Log("File processed: " + file);
                }
                catch (Exception e)
                {
                    updateTableStatus(defaultContext, tableIndex, "An error occured while processing the file.");
                    LoggerFactory.GetLogger().Log("Exception while processing file: " + file, e);
                    //Move the invalid file.
                    string invalidPath = Path.Combine(DirectoryManager.InvalidDir, userName, Path.GetFileName(file));
                    validateDestination(invalidPath);
                    File.Move(file, invalidPath);
                }
            }
        }

        public static bool IsFileReady(string fileName)
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
        private static void validateDestination(string dest)
        {
            string destDir = Directory.GetParent(dest).FullName;
            if (!Directory.Exists(destDir))
                Directory.CreateDirectory(destDir);
            if (File.Exists(dest))
                File.Delete(dest);
        }

        private static void updateTableStatus(DefaultContext context, TableIndex index, string status, bool processed = false)
        {
            index.Status = status;
            index.DateUpdated = DateTime.Now;
            index.Processed = processed;
            context.Entry(index).State = System.Data.EntityState.Modified;
            context.SaveChanges();
        }
    }
}
