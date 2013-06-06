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

        //Allows us to cancel the thread.
        private static bool _cancel;

        /// <summary>
        /// Starts the file processor in a separate thread.
        /// </summary>
        public static void Start()
        {
            _cancel = false;
            Thread t = new Thread(convertFiles);
            t.Start();
        }

        /// <summary>
        /// Stops the file processor. This will not imediatly kill the thread.
        /// </summary>
        public static void Stop()
        {
            _cancel = true;
        }

        /// <summary>
        /// The main method of this class. Until _cancel is set to true it will continually scan the upload directory for any files to process.
        /// </summary>
        private static void convertFiles()
        {
            while (!_cancel)
            {
                try
                {
                    foreach (string userDirectory in Directory.GetDirectories(DirectoryManager.UploadDir))
                    {
                        //See if we need to cancel.
                        if (_cancel)
                            break;

                        string[] files = Directory.GetFiles(userDirectory);

                        //If a lock file exist in the user directory, it mean that the user is uploading file.
                        //We want to wait until the user is done uploading file, so just move on if there is a lock file.
                        string lockFile = Path.Combine(userDirectory, "lock");
                        if (files.Contains(lockFile))
                        {
                            continue;
                        }

                        //Process each file.
                        foreach (string file in files)
                        {
                            if (_cancel)
                                break;

                            //Convert the file.
                            string conversionPath = FileConverterFactory.GetConverter(file).ConvertToCSV();

                            //Add converted file to the database.
                            string userName = Directory.GetParent(file).Name;
                            UserContext context = UserContext.GetContextForUser(userName);
                            context.Commands.AddTableFromFile(conversionPath);
                            context.Commands.PopulateTableFromFile(conversionPath);

                            //Move the original file to the Archive.
                            string archivePath = Path.Combine(DirectoryManager.ArchiveDir, Directory.GetParent(file).Name, Path.GetFileName(file));
                            validateDestination(archivePath);
                            File.Move(file, archivePath);

                            //Log when the file was processed.
                            LoggerFactory.Log("File processed: " + file);
                        }

                        //Release the lock.
                        File.Delete(lockFile);
                        lockFile = null;
                    }
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
                    LoggerFactory.Log("Exception while processing files", e);
                    //Once we hit an exception we don't want to keep going.
                    _cancel = true;
                }
#if DEBUG
                //In debug mode only run once.
                _cancel = true;
#endif
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
