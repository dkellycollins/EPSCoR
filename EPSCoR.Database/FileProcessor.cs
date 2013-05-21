﻿using System;
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

namespace ESPCoR.Database
{
    /// <summary>
    /// The file processor coverts any uploaded files into csv file and moves them to the archive folder.
    /// </summary>
    public class FileProcessor
    {
        //The directories on the server.
        private static string RootDir = HttpContext.Current.Server.MapPath("~/App_Data");
        private static string UploadDir = HttpContext.Current.Server.MapPath("~/App_Data/Uploads");
        private static string ConversionDir = HttpContext.Current.Server.MapPath("~/App_Data/Convertions");
        private static string ArchiveDir = HttpContext.Current.Server.MapPath("~/App_Data/Archive");

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
            if (!Directory.Exists(UploadDir))
                Directory.CreateDirectory(UploadDir);
            if (!Directory.Exists(ConversionDir))
                Directory.CreateDirectory(ConversionDir);
            if (!Directory.Exists(ArchiveDir))
                Directory.CreateDirectory(ArchiveDir);

            while (!_cancel)
            {
                string lockFile = null;
                DefaultContext dbContext = DefaultContext.GetInstance();
                try
                {
                    foreach (string userDirectory in Directory.GetDirectories(UploadDir))
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
                                addTableFromFile(conversionPath, dbContext);
                                populateTableFromFile(conversionPath, dbContext);
                            }

                            //Move the original file to the Archive.
                            string archivePath = Path.Combine(ArchiveDir, Directory.GetParent(file).Name, Path.GetFileName(file));
                            validateDestination(archivePath);
                            File.Move(file, archivePath);

                            //Log when we the file was processed.
                            Log("File processed: " + file);
                        }

                        //Release the lock.
                        File.Delete(lockFile);
                        lockFile = null;
                    }
                }
                catch (Exception e)
                {
                    Log("Exception: " + e.Message);
                }
                finally
                {
                    //This makes sure we release the lock if we were canceled or hit an exception.
                    if (lockFile != null)
                        File.Delete(lockFile);
                    DefaultContext.Release();
                }
#if DEBUG
                _cancel = true;
#endif
            }
        }

        /// <summary>
        /// Populates the table with same name as the file with the data in the file.
        /// </summary>
        /// <param name="file">CSV file</param>
        /// <param name="dbContext">The reference to the database.</param>
        private static void populateTableFromFile(string file, DbContext dbContext)
        {
            string table = Path.GetFileNameWithoutExtension(file);
            
            int rowsUpdated = dbContext.Database.ExecuteSqlCommand(
                "LOAD DATA LOCAL INFILE {0} INTO TABLE {1} FIELDS TERMINATED BY ',' OPTIONALLY ENCLOSED BY '\"' LINES TERMINATED BY '\n' IGNORE 1 LINES",
                file,
                table
                );

            Log(rowsUpdated + " rows updated in table " + table);
        }

        /// <summary>
        /// Creates a new table based on the file provided.
        /// </summary>
        /// <param name="file">CSV file.</param>
        /// <param name="dbContext">Reference to thte database.</param>
        private static void addTableFromFile(string file, DbContext dbContext)
        /*{
            DbProviderFactory dbFactory = DbProviderFactories.GetFactory(dbContext.Database.Connection);
            DbCommand cmd = dbFactory.CreateCommand();
            cmd.Connection = dbContext.Database.Connection;

            DbParameter tableParam = dbFactory.CreateParameter();
            tableParam.ParameterName = "tableName";
            tableParam.Value = Path.GetFileNameWithoutExtension(file);
            cmd.Parameters.Add(tableParam);

            //Get all the fields from the file.
            TextReader reader = File.OpenText(file);
            string head = reader.ReadLine();
            reader.Close();
            head = head.Replace('\"', ' ');

            //Build the column paramaters for the Sql query.
            string[] fields = head.Split(',');
            StringBuilder columnsBuilder = new StringBuilder();
            for (int i = 0; i < fields.Count(); i++)
            {
                columnsBuilder.Append("@column" + i + " char(25), ");

                DbParameter param = dbFactory.CreateParameter();
                param.ParameterName = "column" + i;
                param.Value = fields[i].Trim();
                cmd.Parameters.Add(param);
            }
            //Make the first field the primary key.
            columnsBuilder.Append("PRIMARY KEY(@column0)");

            cmd.CommandText = "CREATE TABLE [IF NOT EXISTS] @tableName (" + columnsBuilder.ToString() + ") ENGINE = InnoDB DEFAULT CHARSET=latin1";

            if (dbContext.Database.Connection.State == System.Data.ConnectionState.Closed)
                dbContext.Database.Connection.Open();
            cmd.ExecuteNonQuery();

            Log("Table " + tableParam.Value + "added to the database.");
        }*/
        {
            //Get all the fields from the file.
            TextReader reader = File.OpenText(file);
            string head = reader.ReadLine();
            reader.Close();
            head = head.Replace('\"', ' ');

            //Build the column paramaters for the Sql query.
            string[] fields = head.Split(',');
            StringBuilder columnsBuilder = new StringBuilder();
            for (int i = 0; i < fields.Count(); i++)
            {
                columnsBuilder.Append(fields[i].Trim() + " char(25), ");
            }
            //Make the first field the primary key.
            columnsBuilder.Append("PRIMARY KEY(" + fields[0].Trim() + ")");
            
            string sqlCommand = "CREATE TABLE [IF NOT EXISTS] " + Path.GetFileNameWithoutExtension(file) + " ( " + columnsBuilder.ToString() + " )";
            dbContext.Database.ExecuteSqlCommand(sqlCommand);

            Log("Table " + Path.GetFileNameWithoutExtension(file) + " added to the database.");
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
            return null;
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
            string conversionPath = Path.Combine(ConversionDir, userDir, fileName);
            validateDestination(conversionPath);
            File.Copy(file, conversionPath);
            return conversionPath;
        }

        /// <summary>
        /// Appends the entry to the log file with the current time.
        /// </summary>
        /// <param name="entry">Log entry.</param>
        private static void Log(string entry)
        {
            string logFile = Path.Combine(RootDir, "Log.txt");
            StreamWriter logFileStream;
            if (!File.Exists(logFile))
                logFileStream = File.CreateText(logFile);
            else
                logFileStream = new StreamWriter(File.Open(logFile, FileMode.Append));

            logFileStream.WriteLine(string.Format("{0} - {1}", DateTime.Now.ToString(), entry));

            logFileStream.Flush();
            logFileStream.Close();
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
