using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using EPSCoR.Database.DbCmds;
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

        private FileSystemWatcher _fileWatcher;

        public FileProcessor()
        {
            _fileWatcher = new FileSystemWatcher(DirectoryManager.UploadDir);
            _fileWatcher.Created += _fileWatcher_Created;

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

        private static void convertFile(string file)
        {
            try
            {
                //Wait until the file can be opened.
                while (!IsFileReady(file)) ;

                //Convert the file.
                string conversionPath = FileConverterFactory.GetConverter(file).ConvertToCSV();

                //Add converted file to the database.
                string userName = Directory.GetParent(file).Name;
                UserContext context = UserContext.GetContextForUser(userName);
                context.Commands.AddTableFromFile(conversionPath);
                context.Commands.PopulateTableFromFile(conversionPath);
                context.Dispose();

                //Move the original file to the Archive.
                string archivePath = Path.Combine(DirectoryManager.ArchiveDir, Directory.GetParent(file).Name, Path.GetFileName(file));
                validateDestination(archivePath);
                File.Move(file, archivePath);

                //Log when the file was processed.
                LoggerFactory.Log("File processed: " + file);
            }
            catch (InvalidFileException e)
            {
                LoggerFactory.Log("Invalid File: " + e.InvalidFile, e);
                //Move the invalid file.
                string invalidPath = Path.Combine(DirectoryManager.InvalidDir, Path.GetFileName(e.InvalidFile));
                validateDestination(invalidPath);
                File.Move(e.InvalidFile, invalidPath);
            }
            catch (Exception e)
            {
                LoggerFactory.Log("Exception while processing file", e);
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
    }
}
