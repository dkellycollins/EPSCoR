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
using EPSCoR.Database.Models;

namespace EPSCoR.Database
{
    /// <summary>
    /// The file processor coverts any uploaded files into csv file and moves them to the archive folder.
    /// </summary>
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
                string lockFile = null;
                try
                {
                    foreach (string userDirectory in Directory.GetDirectories(DirectoryManager.UploadDir))
                    {
                        //See if we need to cancel.
                        if (_cancel)
                            break;

                        //Create a lock file if we can/need to. Otherwise move on.
                        string l = Path.Combine(userDirectory, "lock");
                        string[] files = Directory.GetFiles(userDirectory);
                        if (files.Count() > 0 && !files.Contains(l))
                        {
                            lockFile = l;
                            File.Create(lockFile).Close();
                        }
                        else
                        {
                            continue;
                        }

                        //Process each file.
                        foreach (string file in files)
                        {
                            if (_cancel)
                                break;

                            //Convert the file.
                            string conversionPath;
                            string ext = Path.GetExtension(file).ToLower();
                            switch (ext)
                            {
                                case ".csv":
                                    conversionPath = handleCSV(file);
                                    break;
                                case ".dbf":
                                    conversionPath = handleDBF(file);
                                    break;
                                case ".mdb":
                                    conversionPath = handleMDB(file);
                                    break;
                                default:
                                    conversionPath = null;
                                    break;
                            }

                            //Add converted file to the database.
                            if (conversionPath != null)
                            {
                                MySqlCmd.AddTableFromFile(conversionPath);
                                MySqlCmd.PopulateTableFromFile(conversionPath);
                            }

                            //Move the original file to the Archive.
                            string archivePath = Path.Combine(DirectoryManager.ArchiveDir, Directory.GetParent(file).Name, Path.GetFileName(file));
                            validateDestination(archivePath);
                            File.Move(file, archivePath);

                            //Log when the file was processed.
                            Logger.Log("File processed: " + file);
                        }

                        //Release the lock.
                        File.Delete(lockFile);
                        lockFile = null;
                    }
                }
                catch (InvalidFileException e)
                {
                    Logger.Log("Invalid File: " + e.Message);
                    string invalidPath = Path.Combine(DirectoryManager.InvalidDir, Path.GetFileName(e.InvalidFile));
                    validateDestination(invalidPath);
                    File.Move(e.InvalidFile, invalidPath);
                }
                catch (Exception e)
                {
                    Logger.Log("Exception: " + e.Message);
                }
                finally
                {
                    //This makes sure we release the lock if we were canceled or hit an exception.
                    if (lockFile != null)
                        File.Delete(lockFile);
                }
#if DEBUG
                //In debug mode only run once.
                _cancel = true;
#endif
            }
        }

        

        /// <summary>
        /// Converts mdb into a csv file and stores the conversion in the conversion folder.
        /// </summary>
        /// <param name="file">Path to the file to convert.</param>
        /// <returns>The path to the converted file.</returns>
        private static string handleMDB(string file)
        {
            /*string converstionFile = Path.Combine(ConversionDir, Directory.GetParent(file).Name, Path.GetFileName(file));
            Microsoft.Office.Interop.Access.Application oAccess = new Microsoft.Office.Interop.Access.Application();
            oAccess.OpenCurrentDatabase(file);
            oAccess.RunCommand(Microsoft.Office.Interop.Access.AcCommand.acCmdExportText);

            oAccess.Quit();
            return converstionFile;*/
            throw new NotImplementedException();
        }

        /// <summary>
        /// Converts dbf into a csv file and stores the conversion in the conversion folder.
        /// </summary>
        /// <param name="file">Path to the file to convert.</param>
        /// <returns>The path to the converted file.</returns>
        private static string handleDBF(string file)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Just copies the file to the conversion folder.
        /// </summary>
        /// <param name="file">Path to the file to convert.</param>
        /// <returns>The path to the converted file.</returns>
        private static string handleCSV(string file)
        {
            //CSV files dont need any converstion. Just copy the file to the CnversionDirectory
            string userDir = Directory.GetParent(file).Name;
            string fileName = Path.GetFileName(file);
            string conversionPath = Path.Combine(DirectoryManager.ConversionDir, userDir, fileName);
            validateDestination(conversionPath);
            File.Copy(file, conversionPath);
            return conversionPath;
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
